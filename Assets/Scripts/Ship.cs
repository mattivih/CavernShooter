using UnityEngine;
using System.Collections;
using System;
using ExitGames.Client.Photon;

public class Ship : Photon.PunBehaviour, IPunObservable
{
    #region Public variables
    public static GameObject LocalPlayerInstance;

    public float Rotation, MaxRotation = 30f;
    public float Speed, MaxSpeed, ProjectileSpeed = 50f;
    public float FireRate, DamageMultiplier = 1f;
    public float MaxHealth;
    public float AccelDrag = 1, FreeDrag = 0;
    public float PowerUpEffectYOffSet;


    public GameObject LaserPrefab;
    public GameObject[] Firepoints;
    public GameObject PowerUpPosition;
    public GameObject ShipExplosionPrefab;
    public AudioClip LowHealth;
    public Transform MeshTransform;
    public Material ShipColorMaterial;
    public Color[] ShipColors;
    public ParticleSystem Smoking30;
    public ParticleSystem Smoking60;
    public ParticleSystem Sparks30;
    public ParticleSystem Sparks60;

    #endregion
    #region SyncVars

    //[SyncVar]
    public float Health;
    //[SyncVar(hook = "OnChangeShield")]
    public bool Shield = false;
    #endregion

    #region Private variables
    private int _laserCounter;
    private int shipNumber;
    private float _timer = 0f;
    private bool dead = false;

    private Vector3 _originalMeshRotation;
    private Thruster _thruster;
    private GameObject _lastDamageSource;
    private AudioSource _audioSource;
    private Rigidbody2D _rigid;
    #endregion

    void Awake()
    {
        _thruster = GetComponentInChildren<Thruster>();
        _audioSource = GetComponent<AudioSource>();

        if (photonView.isMine) {
            LocalPlayerInstance = gameObject;
            GameManager.Instance.Player = LocalPlayerInstance;
        }
    }

  
    [PunRPC]
    public void setColors(int viewId)
    {
        GameObject go = PhotonView.Find(viewId).gameObject;
        int ownerid = go.GetPhotonView().ownerId;
        Material newMaterial = new Material(go.GetComponent<Ship>().ShipColorMaterial);

        if (ownerid == 1)
            newMaterial.color = new Color(ShipColors[0].r, ShipColors[0].g, ShipColors[0].b);
        else if (ownerid == 2)
            newMaterial.color = new Color(ShipColors[1].r, ShipColors[1].g, ShipColors[1].b);
        else if (ownerid == 3)
            newMaterial.color = new Color(ShipColors[2].r, ShipColors[2].g, ShipColors[2].b);
        else if (ownerid == 4)
            newMaterial.color = new Color(ShipColors[3].r, ShipColors[3].g, ShipColors[3].b);

        // newMaterial.color = new Color(ShipColors[id - 1].r * 255, ShipColors[id - 1].g * 255, ShipColors[id - 1].b * 255);
        var shipmats = go.GetComponentInChildren<MeshRenderer>().materials;

        for (int i = 0; i < shipmats.Length; i++)
        {
            if (shipmats[i].name == "_Ship_Colour (Instance)" || shipmats[i].name == "_Ship_Colour")
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
        //Debug.LogError("Player: "+ photonView.viewID + " GameManager found: " + GameManager.Instance + " Photonview is mine: " + photonView.isMine);
        if (photonView.isMine)
        {
            Camera.main.GetComponent<CameraController>().FollowShip(transform);
        }
 
        //TODO: refactor to HUD Manager
        GameManager.Instance.powerupBarImage.fillAmount = 0;

        _rigid = GetComponentInChildren<Rigidbody2D>();
        Health = MaxHealth;
        _originalMeshRotation = MeshTransform.localEulerAngles;
        photonView.RPC("setColors", PhotonTargets.All, Ship.LocalPlayerInstance.GetPhotonView().viewID);
    }

    void Start()
    {
        shipNumber = GameManager.Instance.AddShipToList(gameObject);

        if (photonView.isMine)
        {
            foreach (GameObject o in GameObject.FindGameObjectsWithTag("Background"))
            {
                float xOffset = (o.GetComponent<Parallax>().Scale * transform.position.x * -1) / 4;
                float yOffset = (o.GetComponent<Parallax>().Scale * transform.position.y * -1) / 4;
                //Debug.Log("x: " + xOffset + ", y:" + yOffset);
                o.transform.position = new Vector3(xOffset, yOffset, o.transform.position.z);
            }
        }
        //StartCoroutine("TestDying");
    }

  IEnumerator TestDying() {
        yield return new WaitForSeconds(5f);
        GameObject killer = null;
        Ship[] ships = FindObjectsOfType<Ship>();
        foreach (var ship in ships) {
            if (ship.gameObject.GetPhotonView().owner.IsMasterClient) {
                killer = ship.gameObject;
            }
        }
        TakeDamage(1000, killer);
    }

    void Update()
    {
        
        #region Ship damage indicators (smokes and sparks)
        if (Health < (MaxHealth * 0.3f))
        {
            Smoking60.Stop();
            Sparks60.Stop();
            if (!Smoking30.isPlaying)
                Smoking30.Play();
            if (!Sparks30.isPlaying)
                Sparks30.Play();
          /*  if (!_audioSource.isPlaying && _audioSource.clip != LowHealth)
            {
                _audioSource.clip = LowHealth;
                _audioSource.loop = true;
                _audioSource.Play();
            }*/

        }

        else if (Health >= (MaxHealth * 0.3f) && Health <= (MaxHealth * 0.6f))
        {

            Smoking30.Stop();
            Sparks30.Stop();
            if (!Smoking60.isPlaying)
                Smoking60.Play();
            if (!Sparks60.isPlaying)
                Sparks60.Play();
           /* if (_audioSource.isPlaying && _audioSource.clip == LowHealth)
            {
                _audioSource.Stop();
            }*/
        }

        if (Health > (MaxHealth * 0.6f))
        {
            Smoking30.Stop();
            Sparks30.Stop();
            Smoking60.Stop();
            Sparks60.Stop();
        }
        #endregion 

        if (!photonView.isMine && PhotonNetwork.connected)
        {
            tag = "Enemy";
            gameObject.layer = LayerMask.NameToLayer("Enemy"); //enemy layer = 11
            return;
        }

        else
        {

            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            if (vertical > 0)
            {
                Move(vertical);
                GetComponent<Rigidbody2D>().drag = AccelDrag;
                if (_thruster)
                {
                    //TODO: Gives NullreferenceException because using new Particlesystem.MinMaxCurve()
                    photonView.RPC("CmdThrusterOn", PhotonTargets.All);
                }
            }
            else
            {
                GetComponent<Rigidbody2D>().drag = FreeDrag;
                if (_thruster)
                {
                    //TODO: Gives NullreferenceException because using new Particlesystem.MinMaxCurve()
                    photonView.RPC("CmdThrusterOff", PhotonTargets.All);
                }
            }

            ProcessInputs();
            
            #region Bank the ship
            if (horizontal != 0)
            {
                photonView.RPC("BankShip", PhotonTargets.All, horizontal);
            }
            else
            {
                photonView.RPC("ResetShipBank", PhotonTargets.All);
            }
            #endregion
  
        }

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

    [PunRPC]
    void BankShip(float horizontal)
    {
        transform.Rotate(Vector3.forward * Rotation * -horizontal * Time.deltaTime);
        //_rigid.AddTorque(Rotation * -horizontal, ForceMode2D.Force);
        Vector3 bank = _originalMeshRotation;
        bank.y += Mathf.Sign(horizontal) * -MaxRotation;
        MeshTransform.localRotation = Quaternion.Lerp(MeshTransform.localRotation, Quaternion.Euler(bank), Time.deltaTime * (Rotation / 150));
    }

    [PunRPC]
    void ResetShipBank()
    {
        Vector3 bankNone = _originalMeshRotation;
        MeshTransform.localRotation = Quaternion.Lerp(MeshTransform.localRotation, Quaternion.Euler(bankNone), Time.deltaTime * (Rotation / 120));
    }
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

            //Sends request to fire a laser to the server and fires it on all instances of this player
            photonView.RPC("CmdFire", PhotonTargets.All, t.transform.position, t.transform.rotation, GetComponent<Rigidbody2D>().velocity,
                Ship.LocalPlayerInstance.GetPhotonView().viewID);
            _laserCounter++;
        }
    }
    

    //<summary>
    // Photon: Fires a projectile on remote clients.
    // </summary>
    [PunRPC]
    void CmdFire(Vector3 pos, Quaternion rot, Vector2 velocity, int viewID)
    {
        GameObject laser = Instantiate(LaserPrefab, pos, rot);
        laser.GetComponent<Rigidbody2D>().velocity = velocity;
        laser.GetComponent<Rigidbody2D>().AddForce(transform.up * ProjectileSpeed, ForceMode2D.Impulse);

        laser.GetComponent<Laser>().Source = gameObject;

        if (LayerMask.LayerToName(gameObject.layer) == "Enemy") {
            laser.layer = LayerMask.NameToLayer("EnemyProjectiles");
        }
    }

	[PunRPC]
	void DestroyShip(int viewId)
	{
		GameObject explosion = Instantiate(ShipExplosionPrefab, PhotonView.Find(viewId).transform.position + new Vector3(0f, 0f, -1f), Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0, 360))));
	}


    /// <summary>
    /// Decreases the ship's health.
    /// </summary>
    /// <param name="damage">float damage amount</param>
    public void TakeDamage(float damage, GameObject source)
    {

        if (!photonView.isMine || dead)
        {
            return;
        }
       // Debug.Log(gameObject.name + " taking damage for " + damage + " from " + source);


        if (!Shield)
        {
            if (damage > Health)
            {
                dead = true;
                Health = 0;
            }
            else
            {
                Health -= damage;
            }
            GameManager.Instance.UpdateHealthBar(Health, MaxHealth);
        }
        

        if (Health <= 0)
        {
            GameManager.Instance.RemoveShipFromList(shipNumber);
            
            if (source != null && source.GetComponent<Ship>())
            {
                //If killed by another player
                photonView.RPC("PlayerIsDead", PhotonTargets.All, photonView.owner.ID, source.GetPhotonView().ownerId, true);
                GameManager.Instance.SpectateSpecific(source.gameObject, source.GetComponent<Ship>().shipNumber);
            }

            else
            {
                //If killed by something else i. e. environment
                photonView.RPC("PlayerIsDead", PhotonTargets.All, photonView.owner.ID, 0, false);
                GameManager.Instance.SpectateFirst();
            }

            GameManager.Instance.spectating = true;

            GameObject explosion = PhotonNetwork.Instantiate("ShipExlosionPrefab", transform.position + new Vector3(0f, 0f, -1f), Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0, 360))), 0);
            PhotonNetwork.Destroy(gameObject.GetPhotonView());

        }
    }


    [PunRPC]
    public void PlayerIsDead(int playerID, int killerID, bool killedByEnemy)
    {
        PhotonMatchManager.Instance.OnMasterClientOnPlayerDeath(playerID, killerID, killedByEnemy);
    }


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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(Health);
        }
        else
        {
            Health = (float)stream.ReceiveNext();
        }
    }
}
