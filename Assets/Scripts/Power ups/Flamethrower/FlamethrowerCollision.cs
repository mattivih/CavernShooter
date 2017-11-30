using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerCollision : ProjectilesBase {

	public ParticleSystem FlamethrowerFire;
	public GameObject FireEffectPrefab;
    public int ParticleCount;
    public float EnemyParticleCount;
    public float UnitDuration = 0.5f;
    public FireBaseScript currentPrefabScript;

	private GameObject _fire, _flamethrower;
    private bool _isFiring = false;
    List<ParticleCollisionEvent> collisionEvents;
    public FlamethrowerPowerUp _flamethrowerPowerUp;
    public Transform Firepoint;

    public AudioClip clipFire;
    private AudioSource audioFire;

    void Awake() {
        audioFire = AddAudio(clipFire, true, false, 1f);
    }
    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip;
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    /// <summary>
    /// Create list for collision events.
    /// </summary>
    void Start () {
		collisionEvents = new List<ParticleCollisionEvent> ();
    }

    void FixedUpdate()
    {
        if (_isFiring && _flamethrowerPowerUp.Units > 0)
        {
            //Decrease units if the laser is fired continuously
            _flamethrowerPowerUp.Units -= Time.deltaTime / UnitDuration;
            if (_flamethrowerPowerUp.Units <= 0)
                Stop();
            //Debug.Log("flamethrowerunits:" + _flamethrowerPowerUp.Units);
        }
    }

    /// <summary>
    /// Creates an collision event for every 40 collided particles, and calls for a fire effect at the collision event.
    /// </summary>
    /// <param name="other">Other.</param>
    void OnParticleCollision(GameObject other) {
        if(other.transform.root.gameObject.GetComponent<Ship>() && other != transform.root.gameObject)
        {
            gameObject.GetPhotonView().RPC("PunTakeDamageFromFire", PhotonTargets.All, other.gameObject.GetPhotonView().viewID, transform.root.gameObject.GetPhotonView().viewID, Time.deltaTime);
            EnemyParticleCount = EnemyParticleCount +  1.0f * Time.deltaTime;
            if (EnemyParticleCount >= 1.0f)
            {
                ParticlePhysicsExtensions.GetCollisionEvents(FlamethrowerFire, other, collisionEvents);
                for (int i = 0; i < collisionEvents.Count; i++)                          
                    EmitAtLocation(collisionEvents[i], transform.root.gameObject);                  
                
                EnemyParticleCount = 0.0f;
            }

        }           
        else
        {
            ParticleCount++;
            if (ParticleCount > 30)
            {
                ParticlePhysicsExtensions.GetCollisionEvents(FlamethrowerFire, other, collisionEvents);
                for (int i = 0; i < collisionEvents.Count; i++)
                {
                    EmitAtLocation(collisionEvents[i], transform.root.gameObject);
                }
                ParticleCount = 0;
            }
        }
  
	}
    [PunRPC]
    public void SetParentForFire(int childId, int parentId)
    {
        if (PhotonView.Find(parentId).transform.root.GetComponent<Ship>())
            PhotonView.Find(childId).transform.parent = PhotonView.Find(parentId).transform.root;
        else
            PhotonView.Find(childId).transform.parent = PhotonView.Find(parentId).transform;
    }
    [PunRPC]
    public void PunTakeDamageFromFire(int viewId, int sourceId, float deltaTime)
    {
        PhotonView.Find(viewId).GetComponent<Ship>().TakeDamage(Damage * deltaTime, PhotonView.Find(sourceId).gameObject);
    }


	/// <summary>
	/// Creates fire effect at a collision event. Bases and enemy ships take damage.
	/// </summary>
	/// <param name="particleCollisionEvent">Particle collision event.</param>
	void EmitAtLocation (ParticleCollisionEvent particleCollisionEvent, GameObject source) 
	{
		_fire = PhotonNetwork.Instantiate("FlamesEffectsPrefab", particleCollisionEvent.intersection, Quaternion.LookRotation (Vector3.forward, particleCollisionEvent.normal), 0);

		if (particleCollisionEvent.colliderComponent && source != particleCollisionEvent.colliderComponent.gameObject) {
            gameObject.GetPhotonView().RPC("SetParentForFire", PhotonTargets.All, _fire.GetPhotonView().viewID, particleCollisionEvent.colliderComponent.gameObject.GetPhotonView().viewID);
			//_fire.transform.parent = particleCollisionEvent.colliderComponent.transform;

			if (particleCollisionEvent.colliderComponent.transform.gameObject.GetComponent<Ship>() && particleCollisionEvent.colliderComponent.transform.gameObject != source) {
                if (_fire.transform.parent) 
                    _fire.transform.position = _fire.transform.parent.position;
                
			} else if (particleCollisionEvent.colliderComponent.transform.gameObject.tag == "Base") {
				if (_fire.transform.parent)
				_fire.transform.parent.GetComponent<Base> ().TakeDamage (Damage);
			}
		}
		if (_fire.transform.parent == null) 
		{
			PhotonNetwork.Destroy(_fire);
		}
	}

	public void Stop()
	{
		// if we are running a constant effect like wall of fire, stop it now
		if (currentPrefabScript != null)
		{
            _isFiring = false;
            audioFire.Stop();
            currentPrefabScript.Stop();           
		}
        gameObject.GetPhotonView().RPC("DelayPunDestroy", PhotonTargets.All, gameObject.GetPhotonView().viewID);

	}

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            Stop();
    }


    [PunRPC]
    public void DelayPunDestroy(int viewId)
    {
        StartCoroutine("DestroyTimer", viewId);
    }

    IEnumerator DestroyTimer(int viewId)
    {
        yield return new WaitForSeconds(2.0f);
        if (gameObject.GetPhotonView().isMine)
        {
            PhotonNetwork.Destroy(PhotonView.Find(viewId).gameObject.transform.parent.gameObject);
        }
      
    }



	/// <summary>
	/// Starts the flamethrower effect.
	/// </summary>
	/// <param name="flamethrower">Flamethrower.</param>
	public void BeginEffect(GameObject flamethrower)
	{
        _isFiring = true;
        if(audioFire)
        audioFire.Play();
		_flamethrower = flamethrower;

		// set the start point near the player
	}

}
