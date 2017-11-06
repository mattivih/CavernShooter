using UnityEngine;
using System.Collections.Generic;
using System;
using ExitGames.Client.Photon;

public class Ship : Photon.PunBehaviour, IPunObservable
{
    #region Public variables
    public static GameObject LocalPlayerInstance;
    public static int PlayerID;

    public float Rotation, MaxRotation = 30f;
    public float Speed, MaxSpeed, ProjectileSpeed = 50f;
    public float FireRate = 0.1f, DamageMultiplier = 1f;
    public float MaxHealth;
    public float AccelDrag = 1, FreeDrag = 0;
    public float PowerUpEffectYOffSet;


    public GameObject LaserPrefab;
    public GameObject[] Firepoints;
    public GameObject PowerUpPosition;
    public GameObject ShipExplosionPrefab;
    public AudioClip[] MusicArray;
    public Transform MeshTransform;
    public Material ShipColorMaterial;
    public Color[] ShipColors;

    #endregion
    #region SyncVars

    //[SyncVar]
    public float Health;
    //[SyncVar(hook = "OnChangeShield")]
    public float Shield;
    #endregion

    #region Private variables
    private int _laserCounter;

    private float _timer = 0.5f;

    private Vector3 _originalMeshRotation;
    private Thruster _thruster;
    private GameObject _lastDamageSource;
    private AudioSource _music;
    private Rigidbody2D _rigid;
    #endregion

    void Awake()
    {
        _thruster = GetComponentInChildren<Thruster>();

        //Register this player as a local player to this script
        //If we try to register it to the Game Manager here it's too soon and causes errors
        if (photonView.isMine) {
            LocalPlayerInstance = gameObject;
        }
        //Don't destroy on load so the instance survives level loading and the loading between scenes is seamless.
        DontDestroyOnLoad(gameObject);
    }

    //To be deleted: Replaced wiht OnPhotonInstantiate
    //public override void OnStartClient() {
    //    _rigid = GetComponentInChildren<Rigidbody2D>();
    //    Health = MaxHealth;
    //    originalMeshRotation = mesh.localEulerAngles;
    //    //CmdSpawnPlayer(GetComponent<NetworkIdentity>().netId);
    //    playerNum = GameManager.Instance.GetPlayerNum(GetComponent<NetworkIdentity>().netId);

    //    if (playerNum < 4) {
    //        //Assign player's color to ship material
    //        //1. copy original material
    //        Material newMaterial = new Material(shipColorMaterial);
    //        //2. change the copy's colour to ship colour
    //        newMaterial.color = shipColors[playerNum];

    //        //3. find and replace material in ships's renderer's materials
    //        var shipmats = GetComponentInChildren<MeshRenderer>().materials;
    //        for (int i = 0; i < shipmats.Length; i++) {
    //            if (shipmats[i].name == "_Ship_Colour (Instance)") {
    //                shipmats[i] = newMaterial;
    //            }
    //        }
    //        GetComponentInChildren<MeshRenderer>().materials = shipmats;
    //    }
    //}

    //[Command]
    //void CmdSpawnPlayer(NetworkInstanceId id) {
    //    RpcSpawnPlayer(id);
    //}
    //[ClientRpc]
    //void RpcSpawnPlayer(NetworkInstanceId id) {
    //    playerNum = GameManager.Instance.GetPlayerNum(id);
    //}


    //public override void OnStartLocalPlayer() {
  
    [PunRPC]
    public void setColors(int viewId)
    {
        Debug.Log(viewId);
        GameObject go = PhotonView.Find(viewId).gameObject;            
        Material newMaterial = new Material(go.GetComponent<Ship>().ShipColorMaterial);

        if (viewId == 1001)
            newMaterial.color = new Color(ShipColors[0].r * 255, ShipColors[0].g * 255, ShipColors[0].b * 255);
        else if (viewId == 2001)
            newMaterial.color = new Color(ShipColors[1].r * 255, ShipColors[1].g * 255, ShipColors[1].b * 255);
        else if (viewId == 3001)
            newMaterial.color = new Color(ShipColors[2].r * 255, ShipColors[2].g * 255, ShipColors[2].b * 255);
        else if (viewId == 4001)
            newMaterial.color = new Color(ShipColors[3].r * 255, ShipColors[3].g * 255, ShipColors[3].b * 255);

        // newMaterial.color = new Color(ShipColors[id - 1].r * 255, ShipColors[id - 1].g * 255, ShipColors[id - 1].b * 255);
        var shipmats = go.GetComponentInChildren<MeshRenderer>().materials;

        for (int i = 0; i < shipmats.Length; i++)
        {
            if (shipmats[i].name == "_Ship_Colour (Instance)")
            {
                shipmats[i] = newMaterial;
            }
        }

        go.GetComponentInChildren<MeshRenderer>().materials = shipmats;
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
        gameObject.name = "Player " + photonView.viewID;
        //Local client initialization
        Debug.LogError("Player: "+ photonView.viewID + " GameManager found: " + GameManager.Instance + " Photonview is mine: " + photonView.isMine);
        if (photonView.isMine)
        {
            Camera.main.GetComponent<CameraController>().FollowShip(transform);

            //TODO: fix later
            //PlayerID = PhotonNetwork.countOfPlayers;
        }

        //TODO: refactor to HUD Manager
        GameManager.Instance.powerupBarImage.fillAmount = 0;

        _rigid = GetComponentInChildren<Rigidbody2D>();
        Health = MaxHealth;
        _originalMeshRotation = MeshTransform.localEulerAngles;

        #region Assign color to the player
        if (PlayerID < 4)
        {
            //Assign player's color to ship material
            //1. copy original material

            //2. change the copy's colour to ship colour
            if (PlayerID > 0)
            {
                //  newMaterial.color = new Color(ShipColors[PlayerID - 1].r * 255, ShipColors[PlayerID - 1].g * 255, ShipColors[PlayerID - 1].b * 255);
                // photonView.RPC("setColor", PhotonTargets.AllBuffered, PlayerID);
                photonView.RPC("setColors", PhotonTargets.All, Ship.LocalPlayerInstance.GetPhotonView().viewID);

            }

            //3. find and replace material in ships's renderer's materials

        }


        #endregion
    }

    void Start()
    {
        //music = AddAudio(musicArray[Random.Range(0, musicArray.Length)], true, false, 1f);
        //music.Play();

        //Registers the ship to the Game Manager
        GameManager.Instance.Player = gameObject;

        foreach (GameObject o in GameObject.FindGameObjectsWithTag("Background"))
        {
            float xOffset = (o.GetComponent<Parallax>().Scale * transform.position.x * -1) / 4;
            float yOffset = (o.GetComponent<Parallax>().Scale * transform.position.y * -1) / 4;
            //Debug.Log("x: " + xOffset + ", y:" + yOffset);
            o.transform.position = new Vector3(xOffset, yOffset, o.transform.position.z);
        }
    }


    void Update()
    { 
        //TODO : Delete, old Unet code
        //if (!isLocalPlayer)
        //{
        //    tag = "Enemy";
        //    gameObject.layer = 11; //enemy layer
        //    return;
        //}

        if (!photonView.isMine && PhotonNetwork.connected)
        {
            tag = "Enemy";
            gameObject.layer = LayerMask.NameToLayer("Enemy"); //enemy layer = 11
            return;
        } else {

            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            if (vertical > 0)
            {
                Move(vertical);
                GetComponent<Rigidbody2D>().drag = AccelDrag;
                if (_thruster)
                {
                    //TODO: Gives NullreferenceException because using new Particlesystem.MinMaxCurve()
                    //_thruster.ThrusterOn();
                    photonView.RPC("CmdThrusterOn", PhotonTargets.All);

                    //CmdThrusterOn();
                }
            }
            else
            {
                GetComponent<Rigidbody2D>().drag = FreeDrag;
                if (_thruster)
                {
                    //TODO: Gives NullreferenceException because using new Particlesystem.MinMaxCurve()
                    //_thruster.ThrusterOff();
                    photonView.RPC("CmdThrusterOff", PhotonTargets.All);
                    //CmdThrusterOff();
                }
            }

            #region Bank the ship
            if (horizontal != 0)
            {
                transform.Rotate(Vector3.forward * Rotation * -horizontal * Time.deltaTime);
                //_rigid.AddTorque(Rotation * -horizontal, ForceMode2D.Force);
                Vector3 bank = _originalMeshRotation;
                bank.y += Mathf.Sign(horizontal) * -MaxRotation;
                MeshTransform.localRotation = Quaternion.Lerp(MeshTransform.localRotation, Quaternion.Euler(bank), Time.deltaTime * (Rotation / 150));
            }
            else
            {
                Vector3 bankNone = _originalMeshRotation;
                MeshTransform.localRotation = Quaternion.Lerp(MeshTransform.localRotation, Quaternion.Euler(bankNone), Time.deltaTime * (Rotation / 120));
            }
            #endregion
            #region To be deleted: Code moved to function ProcessInputs()
            ////Fire normal weapon
            //if (Input.GetKey(KeyCode.Period))
            //{
            //    if (_timer > FireRate)
            //    {
            //        Fire();
            //        _timer = 0;
            //    }
            //    _timer += Time.deltaTime;
            //}
            //if (Input.GetKeyUp(KeyCode.Period))
            //{
            //    _timer = FireRate;
            //}

            ////Use Power-Up
            //if (Input.GetKeyDown(KeyCode.Comma))
            //{
            //    //GetComponent<PowerUpHandler>().Use();
            //}
            //if (Input.GetKeyUp(KeyCode.Comma))
            //{
            //    //GetComponent<PowerUpHandler>().Stop();
            //}
            #endregion

            ProcessInputs();
        }

        //TODO: refactor to HUDManager
        #region Refactor to HUDManager
        if (GameManager.Instance.healthbarImage)
        {
            float healthFraction = (Health / MaxHealth);
            healthFraction *= 0.92f;
            healthFraction += 0.04f;
            GameManager.Instance.healthbarImage.fillAmount = healthFraction;
            if (healthFraction > 0.5)
            {
                GameManager.Instance.healthbarImage.color = Color.Lerp(Color.yellow, Color.green, (healthFraction - 0.5f) * 2);
            }
            else
            {
                GameManager.Instance.healthbarImage.color = Color.Lerp(Color.red, Color.yellow, healthFraction * 2);
            }
        }
        if (GameManager.Instance.shieldbarImage)
        {
            GameManager.Instance.shieldbarImage.fillAmount = Shield / MaxHealth;
        }
        /*if (GameManager.Instance.powerupBarImage)
        {
            GameObject CurrentPowerUp = GetComponent<PowerUpHandler>().CurrentPowerUp;
            if (CurrentPowerUp)
            {
                if (CurrentPowerUp.GetComponent<PowerUp>().isUsed)
                      {
                          float waitTime = 15f;
                          GameManager.Instance.powerupBarImage.fillAmount -= 1.0f / waitTime * Time.deltaTime;

                      }

                      else
                      {
                          float powerUpFraction = CurrentPowerUp.GetComponent<PowerUp>().Units / CurrentPowerUp.GetComponent<PowerUp>().MaxUnits;
                          GameManager.Instance.powerupBarImage.fillAmount = powerUpFraction;
                      }

                      if (CurrentPowerUp.GetComponent<PowerUp>().MaxUnits == 3)
                      {
                          GameManager.Instance.powerupBarLines.enabled = true;
                      }
                      else
                      {
                          GameManager.Instance.powerupBarLines.enabled = false;
                      }
                    if (CurrentPowerUp.GetComponent<PowerUp>().MaxUnits == 4)
                    GameManager.Instance.powerupBarLines4.enabled = true;
                else
                    GameManager.Instance.powerupBarLines4.enabled = false;
            }
        }*/
    #endregion
    }

    //Handles user input
    void ProcessInputs() {
        //Fire normal weapon
        if (Input.GetKey(KeyCode.Period))
        {
            if (_timer > FireRate)
            {
                Fire();
                _timer = 0;
            }
            _timer += Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Period))
        {
            _timer = FireRate;
        }

        //Use Power-Up
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            GetComponent<PowerUpHandler>().Use();
        }
        if (Input.GetKeyUp(KeyCode.Comma))
        {
            GetComponent<PowerUpHandler>().Stop();
        }
    }

    #region PUN RPCs - methods that are called on all clients
    [PunRPC]
    void CmdThrusterOn()
    {
        _thruster.ThrusterOn();
    }

    [PunRPC]
    void CmdThrusterOff()
    {
        _thruster.ThrusterOff();
    }

    #endregion

    #region To be deleted: Old Unet methods 

    //[Command]
    //void CmdThrusterOn() {
    //    RpcThrusterOn();
    //}
    //[ClientRpc]
    //void RpcThrusterOn() {
    //    ThrusterScript.ThrusterOn();
    //}
    //[Command]
    //void CmdThrusterOff() {
    //    RpcThrusterOff();
    //}
    //[ClientRpc]
    //void RpcThrusterOff() {
    //    ThrusterScript.ThrusterOff();
    //}
    #endregion

    /// <summary>
    /// Moves the ship.
    /// </summary>
    void Move(float mult)
    {
        float vel = transform.InverseTransformDirection(GetComponent<Rigidbody2D>().velocity).sqrMagnitude;
        //Debug.Log(vel);
        if (vel < MaxSpeed)
        {
            _rigid.AddForce(transform.up * Speed * mult *Time.deltaTime * 20, ForceMode2D.Force);
        }
    }

    void Fire()
    {
        foreach (GameObject t in Firepoints)
        {
            // Creates a dummy laser on local client without a collider.
            //GameObject laser = Instantiate(LaserPrefab, t.transform.position, t.transform.rotation);
            //laser.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
            //laser.GetComponent<Rigidbody2D>().AddForce(transform.up * ProjectileSpeed, ForceMode2D.Impulse);
            //laser.name = "Clientlaser " + _laserCounter;
            //laser.GetComponent<Laser>().clientSide = true;
            //laser.GetComponent<Laser>().myColor = ShipColors[PlayerID - 1];
            //laser.GetComponent<Collider2D>().enabled = false;

            //Sends request to fire a laser to the server and fires it on all instances of this player
            photonView.RPC("CmdFire", PhotonTargets.All, t.transform.position, t.transform.rotation, GetComponent<Rigidbody2D>().velocity, PlayerID);
            //CmdFire(GetComponent<NetworkIdentity>().netId, t.transform.position, t.transform.rotation, laser.GetComponent<Rigidbody2D>().velocity, laserCounter, color);
            _laserCounter++;
        }
    }

    /// <summary>
    /// Unet: Fires a projectile.
    /// </summary>
    //[Command]
    //TODO: Refactor with [PunRPC]
    //void CmdFire(NetworkInstanceId id, Vector3 pos, Quaternion rot, Vector2 velocity, int laserid, Color color) {
    //    GameObject laser = Instantiate(LaserPrefab, pos, rot);
    //    laser.GetComponent<Rigidbody2D>().velocity = velocity;
    //    laser.GetComponent<Rigidbody2D>().AddForce(transform.up * ProjectileSpeed, ForceMode2D.Impulse);
    //    laser.GetComponent<Laser>().myColor = color;
    //    NetworkServer.Spawn(laser);
    //    RpcFire(id, laser, laserid);
    //}


    //<summary>
    // Photon: Fires a projectile on remote clients.
    // </summary>
    [PunRPC]
    void CmdFire(Vector3 pos, Quaternion rot, Vector2 velocity, int playerID)
    {
        //Debug.LogError("Player " + playerID + " on client " + PlayerID + " receiving RPC for firing laser");
        GameObject laser = Instantiate(LaserPrefab, pos, rot);
        laser.GetComponent<Rigidbody2D>().velocity = velocity;
        laser.GetComponent<Rigidbody2D>().AddForce(transform.up * ProjectileSpeed, ForceMode2D.Impulse);
        
        //Color is not serialized, can't send it as RPC argument
        //laser.GetComponent<Laser>().myColor = color;
        laser.GetComponent<Laser>().myColor = ShipColors[playerID - 1];

        laser.GetComponent<Laser>().Source = gameObject;

        if (LayerMask.LayerToName(gameObject.layer) == "Enemy") {
            laser.layer = LayerMask.NameToLayer("EnemyProjectiles");
        }
    }

	[PunRPC]
	void DestroyShip(Vector3 position)
	{
		GameObject explosion = Instantiate(ShipExplosionPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0, 360))));
	}

    //[ClientRpc]
    //void RpcFire(NetworkInstanceId id, GameObject projectile, int laserid) {
    //    projectile.name = "Serverlaser " + laserid;
    //    GameObject player = ClientScene.FindLocalObject(id);
    //    //TODO: gives null reference error
    //    projectile.GetComponent<Laser>().Source = player;
    //    if (player != GameManager.Instance.Player) {
    //        projectile.layer = 12;
    //    } else {
    //        projectile.GetComponent<Renderer>().enabled = false;
    //        projectile.GetComponent<SpriteRenderer>().material.SetFloat("_MKGlowPower", 0f);

    //        //projectile.GetComponent<SpriteRenderer>().material.color = Color.red;
    //        GameObject clientlaser = GameObject.Find("Clientlaser " + laserid);

    //        if (clientlaser) {
    //            clientlaser.GetComponent<Laser>().SetServerObj(projectile);
    //        }
    //    }
    //}

    /// <summary>
    /// Decreases the ship's health.
    /// </summary>
    /// <param name="damage">float damage amount</param>
    public void TakeDamage(float damage, GameObject source)
    {
        if (!photonView.isMine)
        {
            return;
        }

        //Debug.Log(gameObject.name + " taking damage for " + damage + " from " + source);

        if (Shield > 0)
        {
            if (damage < Shield)
            {
                Shield -= damage;
                damage = 0;
            }
            else
            {
                damage -= Shield;
                Shield = 0;
            }
            GameManager.Instance.UpdateShieldBar(Shield, MaxHealth);
        }
        Health -= damage;
     //   Debug.LogError("Player " + PlayerID + " health: " + Health);
        GameManager.Instance.UpdateHealthBar(Health, MaxHealth);


        if (Health <= 0)
        {
            //Is Dead
            //GameManager.Instance.players.Remove(GetComponent<NetworkIdentity>().netId);
            //CmdInformServerPlayerIsDead(gameObject.name);

            // Checks if damage was dealt by a ship. If yes, follow the killer's camera.
            if (source != null && source.GetComponent<Ship>())
            {
                Camera.main.GetComponent<CameraController>().FollowShip(source.transform);
            }
            //Destroy(gameObject);
            //GameObject explosion = Instantiate(ShipExplosionPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0, 360))));
            //NetworkServer.Spawn(explosion);
            //NetworkServer.Destroy(gameObject);
            //Destroy(gameObject);

			photonView.RPC("DestroyShip", PhotonTargets.All, transform.position);
			PhotonNetwork.Destroy (gameObject);
        }
    }

    /// <summary>
    /// Saves player's name to server.
    /// </summary>
    //[Command]
    //TODO: Refactor with [PunRPC]
    //public void CmdInformServerPlayerIsDead(string playerName) {
    //    if (MyLobbyManager.Instance) {
    //        MyLobbyManager.Instance.OnServerOnPlayerDeath(playerName);
    //    }
    //}

    /// <summary>
    /// Increases ship's health when it lands on a base (or via powerup).
    /// </summary>
    /// <param name="amount">Amount of health</param>
    //[ClientRpc]
    //public void RpcIncreaseHealth(float amount) {
    //    Health += amount;
    //    if (Health > MaxHealth) {
    //        Health = MaxHealth;
    //    }
    //}

    public void IncreaseHealth(float amount)
    {
        Health += amount;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    void OnChangeShield(float value)
    {
        if (value == 0)
        {
            GetComponentInChildren<ShieldEffect>().Die();
        }
    }

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip;
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    /// <summary>
    /// Send data over the network
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //This is local player: send other players our stats
            stream.SendNext(Health);
        }
        else
        {
            //This is remote player: receive data
            Health = (float)stream.ReceiveNext();
        }
    }
}
