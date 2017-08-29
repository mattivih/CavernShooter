using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerCollision : ProjectilesBase {

	public ParticleSystem FlamethrowerFire;
	public GameObject FireEffectPrefab;
    public int ParticleCount;
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
        if (_isFiring)
        {
            //Decrease units if the laser is fired continuously
            _flamethrowerPowerUp.Units -= Time.deltaTime / UnitDuration;
            //Debug.Log("flamethrowerunits:" + _flamethrowerPowerUp.Units);
        }
    }

    /// <summary>
    /// Creates an collision event for every 40 collided particles, and calls for a fire effect at the collision event.
    /// </summary>
    /// <param name="other">Other.</param>
    void OnParticleCollision(GameObject other) {
		ParticleCount++;
		if (ParticleCount > 30) {
			ParticlePhysicsExtensions.GetCollisionEvents (FlamethrowerFire, other, collisionEvents);
			for (int i = 0; i < collisionEvents.Count; i++) {
				EmitAtLocation (collisionEvents [i]);
			}
			ParticleCount = 0;
		}
	}

	/// <summary>
	/// Creates fire effect at a collision event. Bases and enemy ships take damage.
	/// </summary>
	/// <param name="particleCollisionEvent">Particle collision event.</param>
	void EmitAtLocation (ParticleCollisionEvent particleCollisionEvent) 
	{
		_fire = Instantiate(FireEffectPrefab, particleCollisionEvent.intersection, Quaternion.LookRotation (Vector3.forward, particleCollisionEvent.normal));

		if (particleCollisionEvent.colliderComponent) {
			_fire.transform.parent = particleCollisionEvent.colliderComponent.transform;

			if (particleCollisionEvent.colliderComponent.transform.gameObject.tag == "Enemy") {
                if (_fire.transform.parent) {
                    _fire.transform.parent.GetComponent<Ship>().TakeDamage(Damage, transform.root.gameObject);
                }
			} else if (particleCollisionEvent.colliderComponent.transform.gameObject.tag == "Base") {
				if (_fire.transform.parent)
				_fire.transform.parent.GetComponent<Base> ().TakeDamage (Damage);
			}
		}
		if (_fire.transform.parent == null) 
		{
			Destroy (_fire);
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

	}

	/// <summary>
	/// Starts the flamethrower effect.
	/// </summary>
	/// <param name="flamethrower">Flamethrower.</param>
	public void BeginEffect(GameObject flamethrower)
	{
        _isFiring = true;
        audioFire.Play();
		_flamethrower = flamethrower;

		// set the start point near the player
	}

}
