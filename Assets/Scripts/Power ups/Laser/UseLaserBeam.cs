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

    private bool _haveSparks = false;
    private bool _isFiring;
    private LineRenderer _lineRenderer;
    public Vector3 _endPoint;

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

    [PunRPC]
    public void FireLaser(Vector3 endpoint)
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, Firepoint.position);
        _lineRenderer.SetPosition(1, endpoint);

    }
    public void CreateSparks()
    {
        sparksObject = PhotonNetwork.Instantiate("Sparks", _endPoint, Quaternion.identity, 0);
        sparkParticleSystem = sparksObject.GetComponent<ParticleSystem>();
        sparkParticleEmission = sparkParticleSystem.emission;
        sparkEmissionNone.constantMax = 0;
        _haveSparks = true;
    }

    [PunRPC]
    public void MoveSparks(int viewId, Vector3 endpoint)
    {
        GameObject s = PhotonView.Find(viewId).gameObject;
        s.transform.position = endpoint;
        s.transform.rotation = Quaternion.identity;
    }

    void Update() {
        if (_isFiring && LaserPowerUp.Units > 0 && GetComponent<PhotonView>().photonView.isMine)
        {
            if (!_haveSparks)            
                CreateSparks();                         
            else           
                GetComponent<PhotonView>().RPC("MoveSparks", PhotonTargets.All, sparksObject.GetComponent<PhotonView>().viewID, _endPoint); 
                          
            gameObject.GetComponent<PhotonView>().RPC("FireLaser", PhotonTargets.All, _endPoint);
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
        if (sparksObject)
        {
            _isFiring = false;
            PhotonNetwork.Destroy(sparksObject);
            _haveSparks = false;
            sparksObject = null;
        }
        GetComponent<Renderer>().enabled = false;
        audioFire.Stop();
        PhotonNetwork.Destroy(gameObject);
    }

}
