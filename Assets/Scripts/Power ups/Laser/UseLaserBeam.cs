using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseLaserBeam : MonoBehaviour {

    public float Duration = 0.5f;
    [Tooltip("Max possible distance of the laser ray")]
    public float MaxDistance = 1000f;
    [Tooltip("How many seconds of firing the laser corresponds to one unit")]
    public float UnitDuration = 0.5f;
    [HideInInspector]
    public LaserBeamPowerUp LaserPowerUp;
    [HideInInspector]
    public Transform Firepoint;

	public LayerMask LayerMaskPlayer;
	public LayerMask LayerMaskEnemy;
	public LayerMask LayerMask;

    public GameObject SparksPrefab;
    GameObject sparksObject;
    ParticleSystem sparkParticleSystem;
    ParticleSystem.EmissionModule sparkParticleEmission;
    ParticleSystem.MinMaxCurve sparkEmissionNone = new ParticleSystem.MinMaxCurve();

    public float DPS;

    private bool _isFiring;
    private LineRenderer _lineRenderer;

    public AudioClip clipFire, clipHitPlayer;
    private AudioSource audioFire, audioHitPlayer;

    void Awake() {
        audioFire = AddAudio(clipFire, true, false, 1f);
        audioHitPlayer = AddAudio(clipHitPlayer, false, false, 1f);
    }

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip;
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }


    void Update() {
        if (_isFiring) {
            //Find endpoint for the laser ray
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, Firepoint.position);
            _lineRenderer.SetPosition(1, FindEndpoint());

            //Decrease units if the laser is fired continuously
            LaserPowerUp.Units -= Time.deltaTime / UnitDuration;
        }
    }

    /// <summary>
    /// Draws the laser
    /// </summary>
    public void Fire() {
        _isFiring = true;
        audioFire.Play();
    }

    /// <summary>
    /// Disables the laser
    /// </summary>
    public void Stop() {
        if (sparksObject) {
            sparksObject.GetComponent<Destroyer>().DestroyDelayed(sparkParticleSystem.main.startLifetime.constantMax);
            sparkParticleEmission.rateOverTime = sparkEmissionNone;
            sparksObject = null;
        }
        GetComponent<Renderer>().enabled = false;
        _isFiring = false;
        audioFire.Stop();
    }

    /// <summary>
    /// Finds the end point for the laser ray
    /// </summary>
    Vector3 FindEndpoint() {
        Vector3 endPoint = Vector3.zero;
		RaycastHit2D hit;
		if (hit = Physics2D.Raycast(Firepoint.position, Firepoint.up, MaxDistance, LayerMask)) {
            endPoint = hit.point;
            if (hit.collider.GetComponent<Ship>()) {
                hit.collider.GetComponent<Ship>().TakeDamage(DPS * Time.deltaTime, transform.root.gameObject);
                AudioSource.PlayClipAtPoint(clipHitPlayer, hit.point);
            } else if (hit.collider.GetComponent<Base>()) {
                hit.collider.GetComponent<Base>().TakeDamage(DPS * Time.deltaTime);
            }
        }

        if (!sparksObject) {
            sparksObject = Instantiate(SparksPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            sparkParticleSystem = sparksObject.GetComponent<ParticleSystem>();
            sparkParticleEmission = sparkParticleSystem.emission;
            sparkEmissionNone.constantMax = 0;
        } else {
            sparksObject.transform.position = hit.point;
            sparksObject.transform.rotation = Quaternion.LookRotation(hit.normal);
        }

        return endPoint;
    }

    public void SetParent(Transform parent) {
        transform.parent = parent;
    }
}
