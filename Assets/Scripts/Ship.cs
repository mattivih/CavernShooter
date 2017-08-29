using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Ship : NetworkBehaviour {
    public float Rotation, MaxRotation = 30f, Speed, ProjectileSpeed = 50f, FireRate = 0.1f, MaxSpeed, DamageMultiplier = 1f;
    public float MaxHealth;
    public float AccelDrag = 1, FreeDrag = 0;
    public float PowerUpEffectYOffSet;
    public GameObject LaserPrefab;
    public GameObject[] Firepoints;
    public GameObject PowerUpPosition;
    public Transform mesh;
    public GameObject ShipExplosionPrefab;
    public int playerNum = 0;
    public Material shipColorMaterial;
    public Color[] shipColors;

    private Vector3 originalMeshRotation;
    private float timer = 0.5f;
    private Thruster ThrusterScript;
    private GameObject lastDamageSource;

    public AudioClip[] musicArray;
    private AudioSource music; 


    [SyncVar]
    public float Health;
    [SyncVar(hook = "OnChangeShield")]
    public float Shield;

    private Rigidbody2D _rigid;

    private int laserCounter;


    void Awake() {
        ThrusterScript = GetComponentInChildren<Thruster>();
    }

    public override void OnStartClient() {
        _rigid = GetComponentInChildren<Rigidbody2D>();
        Health = MaxHealth;
        originalMeshRotation = mesh.localEulerAngles;
        //CmdSpawnPlayer(GetComponent<NetworkIdentity>().netId);
        playerNum = GameManager.Instance.GetPlayerNum(GetComponent<NetworkIdentity>().netId);

        if (playerNum < 4) {
            //Assign player's color to ship material
            //1. copy original material
            Material newMaterial = new Material(shipColorMaterial);
            //2. change the copy's colour to ship colour
            newMaterial.color = shipColors[playerNum];

            //3. find and replace material in ships's renderer's materials
            var shipmats = GetComponentInChildren<MeshRenderer>().materials;
            for (int i = 0; i < shipmats.Length; i++) {
                if (shipmats[i].name == "_Ship_Colour (Instance)") {
                    shipmats[i] = newMaterial;
                }
            }
            GetComponentInChildren<MeshRenderer>().materials = shipmats;
        }
    }

    [Command]
    void CmdSpawnPlayer(NetworkInstanceId id) {
        RpcSpawnPlayer(id);
    }
    [ClientRpc]
    void RpcSpawnPlayer(NetworkInstanceId id) {
        playerNum = GameManager.Instance.GetPlayerNum(id);
    }


    public override void OnStartLocalPlayer() {

        music = AddAudio(musicArray[Random.Range(0, musicArray.Length)], true, false, 1f);
        music.Play();

        Camera.main.GetComponent<CameraController>().Ship = gameObject;
        //Registers the ship to the Game Manager
        GameManager.Instance.Player = gameObject;
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("Background")) {
            float xOffset = (o.GetComponent<Parallax>().Scale * transform.position.x * -1) / 4;
            float yOffset = (o.GetComponent<Parallax>().Scale * transform.position.y * -1) / 4;
            //Debug.Log("x: " + xOffset + ", y:" + yOffset);
            o.transform.position = new Vector3(xOffset, yOffset, o.transform.position.z);
        }
    }



    void Update() {
        if (!isLocalPlayer) {
            tag = "Enemy";
            gameObject.layer = 11; //enemy layer
            return;
        }

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (vertical > 0) {
            Move(vertical);
            GetComponent<Rigidbody2D>().drag = AccelDrag;
            //thrusteron
            if (ThrusterScript) {
                ThrusterScript.ThrusterOn();
                CmdThrusterOn();
            }
        } else {
            GetComponent<Rigidbody2D>().drag = FreeDrag;
            //thrusteroff
            if (ThrusterScript) {
                ThrusterScript.ThrusterOff();
                CmdThrusterOff();
            }
        }

        //TODO: Rotates ship too quickly in build
        // Make not dependent on framerate 
        if (horizontal != 0) {
            transform.Rotate(Vector3.forward * Rotation * -horizontal * Time.deltaTime);
            //_rigid.AddTorque(Rotation * -horizontal, ForceMode2D.Force);
            Vector3 bank = originalMeshRotation;
            bank.y += Mathf.Sign(horizontal) * -MaxRotation;
            mesh.localRotation = Quaternion.Lerp(mesh.localRotation, Quaternion.Euler(bank), Time.deltaTime * (Rotation / 150));
        } else {
            Vector3 bankNone = originalMeshRotation;
            mesh.localRotation = Quaternion.Lerp(mesh.localRotation, Quaternion.Euler(bankNone), Time.deltaTime * (Rotation / 120));
        }
        if (Input.GetKey(KeyCode.Period)) {
            if (timer > FireRate) {
                Fire();
                timer = 0;
            }
            timer += Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.Period)) {
            timer = FireRate;
        }
        if (Input.GetKeyDown(KeyCode.Comma)) {
            GetComponent<PowerUpHandler>().Use();
        }
        if (Input.GetKeyUp(KeyCode.Comma)) {
            GetComponent<PowerUpHandler>().Stop();
        }

        if (GameManager.Instance.healthbarImage) {
            float healthFraction = (Health / MaxHealth);
            healthFraction *= 0.92f;
            healthFraction += 0.04f;
            GameManager.Instance.healthbarImage.fillAmount = healthFraction;
            if (healthFraction > 0.5) {
                GameManager.Instance.healthbarImage.color = Color.Lerp(Color.yellow, Color.green, (healthFraction - 0.5f) * 2);
            } else {
                GameManager.Instance.healthbarImage.color = Color.Lerp(Color.red, Color.yellow, healthFraction * 2);
            }
        }
        if (GameManager.Instance.shieldbarImage) {
            GameManager.Instance.shieldbarImage.fillAmount = Shield / MaxHealth;
        }
        if (GameManager.Instance.powerupBarImage) {
            GameObject CurrentPowerUp = GetComponent<PowerUpHandler>().CurrentPowerUp;
            if (CurrentPowerUp) {
                float powerUpFraction = CurrentPowerUp.GetComponent<PowerUp>().Units / CurrentPowerUp.GetComponent<PowerUp>().MaxUnits;
                GameManager.Instance.powerupBarImage.fillAmount = powerUpFraction;
                if (CurrentPowerUp.GetComponent<PowerUp>().MaxUnits == 3) {
                    GameManager.Instance.powerupBarLines.enabled = true;
                } else {
                    GameManager.Instance.powerupBarLines.enabled = false;
                }
            }
        }

    }

    [Command]
    void CmdThrusterOn() {
        RpcThrusterOn();
    }
    [ClientRpc]
    void RpcThrusterOn() {
        ThrusterScript.ThrusterOn();
    }
    [Command]
    void CmdThrusterOff() {
        RpcThrusterOff();
    }
    [ClientRpc]
    void RpcThrusterOff() {
        ThrusterScript.ThrusterOff();
    }

    /// <summary>
    /// Moves the ship.
    /// </summary>
    void Move(float mult) {
        float vel = transform.InverseTransformDirection(GetComponent<Rigidbody2D>().velocity).sqrMagnitude;
        //Debug.Log(vel);
        if (vel < MaxSpeed) {
            _rigid.AddForce(transform.up * Speed * mult, ForceMode2D.Force);
        }
    }

    void Fire() {
        foreach (GameObject t in Firepoints) {
            GameObject laser = Instantiate(LaserPrefab, t.transform.position, t.transform.rotation);
            laser.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
            laser.GetComponent<Rigidbody2D>().AddForce(transform.up * ProjectileSpeed, ForceMode2D.Impulse);
            laser.name = "Clientlaser " + laserCounter;
            laser.GetComponent<Laser>().clientSide = true;
            laser.GetComponent<Laser>().myColor = shipColors[playerNum];
            laser.GetComponent<Collider2D>().enabled = false;
            Color color = shipColors[playerNum];

            CmdFire(GetComponent<NetworkIdentity>().netId, t.transform.position, t.transform.rotation, laser.GetComponent<Rigidbody2D>().velocity, laserCounter, color);
            laserCounter++;
        }
    }

    /// <summary>
    /// Fires a projectile.
    /// </summary>
    [Command]
    void CmdFire(NetworkInstanceId id, Vector3 pos, Quaternion rot, Vector2 velocity, int laserid, Color color) {
        GameObject laser = Instantiate(LaserPrefab, pos, rot);
        laser.GetComponent<Rigidbody2D>().velocity = velocity;
        laser.GetComponent<Rigidbody2D>().AddForce(transform.up * ProjectileSpeed, ForceMode2D.Impulse);
        laser.GetComponent<Laser>().myColor = color;
        NetworkServer.Spawn(laser);
        RpcFire(id, laser, laserid);

    }

    [ClientRpc]
    void RpcFire(NetworkInstanceId id, GameObject projectile, int laserid) {
        projectile.name = "Serverlaser " + laserid;
        GameObject player = ClientScene.FindLocalObject(id);
        //TODO: gives null reference error
        projectile.GetComponent<Laser>().Source = player;
        if (player != GameManager.Instance.Player) {
            projectile.layer = 12;
        } else {
            projectile.GetComponent<Renderer>().enabled = false;
            projectile.GetComponent<SpriteRenderer>().material.SetFloat("_MKGlowPower", 0f);

            //projectile.GetComponent<SpriteRenderer>().material.color = Color.red;
            GameObject clientlaser = GameObject.Find("Clientlaser " + laserid);

            if (clientlaser) {
                clientlaser.GetComponent<Laser>().SetServerObj(projectile);
            }
        }
    }

    /// <summary>
    /// Decreases the ship's health.
    /// </summary>
    /// <param name="damage">float damage amount</param>
    public void TakeDamage(float damage, GameObject source) {
        if (!isServer) {
            return;
        }
        //Debug.Log(gameObject.name + " taking damage for " + damage + " from " + source);
        lastDamageSource = source;
        if (Shield > 0) {
            if (damage < Shield) {
                Shield -= damage;
                damage = 0;
            } else {
                damage -= Shield;
                Shield = 0;
            }
        }
        Health -= damage;
        if (Health <= 0) {
            //Is Dead
            GameManager.Instance.players.Remove(GetComponent<NetworkIdentity>().netId);
            CmdInformServerPlayerIsDead(gameObject.name);

            // Checks if damage was dealt by a ship.
            // If yes, follow the killer's camera.
            if (lastDamageSource != null && lastDamageSource.GetComponent<Ship>()) {
                Camera.main.GetComponent<CameraController>().Ship = lastDamageSource;
            }
            //Destroy(gameObject);
            GameObject explosion = (GameObject)Instantiate(ShipExplosionPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))));
            NetworkServer.Spawn(explosion);
            NetworkServer.Destroy(gameObject);
            //Destroy(gameObject);

        }
    }

    /// <summary>
    /// Saves player's name to server.
    /// </summary>
    [Command]
    public void CmdInformServerPlayerIsDead(string playerName) {
        if (MyLobbyManager.Instance) {
            MyLobbyManager.Instance.OnServerOnPlayerDeath(playerName);
        }
    }

    /// <summary>
    /// Increases ship's health when it lands on a base (or via powerup).
    /// </summary>
    /// <param name="amount">Amount of health</param>
    [ClientRpc]
    public void RpcIncreaseHealth(float amount) {
        Health += amount;
        if (Health > MaxHealth) {
            Health = MaxHealth;
        }
    }

    void OnChangeShield(float value) {
        if (value == 0) {
            GetComponentInChildren<ShieldEffect>().Die();
        }
    }

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip;
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }
}
