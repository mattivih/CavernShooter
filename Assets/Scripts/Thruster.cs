using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour {

    ParticleSystem thruster;
    public float MaxThrusterVelocity;
    private ParticleSystem.VelocityOverLifetimeModule _velocityOverLifetime;
    private ParticleSystem.EmissionModule _emission;
    private ParticleSystem.MinMaxCurve _thrusterOnOff = new ParticleSystem.MinMaxCurve();
    private ParticleSystem.MinMaxCurve _emissionRate = new ParticleSystem.MinMaxCurve();
    private ParticleSystem.MinMaxCurve _originalEmissionRate;
    [SerializeField]
    AnimationCurve curve;
    [SerializeField]
    float timeToMax;
    float timer=0;

    public AudioClip clipThruster;
    private AudioSource audioThruster;
    float audioFraction = 0f;

    void Awake() {
        thruster = GetComponentInChildren<ParticleSystem>();
        audioThruster = AddAudio(clipThruster, true, false, 0f);

        if (GetComponentInParent<PhotonView>().isMine)
            audioThruster.Play();

        _velocityOverLifetime = thruster.velocityOverLifetime;
        _emission = thruster.emission;
        _originalEmissionRate = thruster.emission.rateOverTime;
    }
    void Start() {

    }

    void Update() {
        //float frac = _emission.rateOverTime.constantMax / _originalEmissionRate.constantMax;
        //Debug.Log(audioFraction);
        audioThruster.volume = audioFraction;
        audioThruster.pitch = audioFraction+0.5f;
    }

    public void ThrusterOn() {
        timer += Time.deltaTime;
        _thrusterOnOff.constantMax = Mathf.Lerp(_thrusterOnOff.constantMax, MaxThrusterVelocity, Time.deltaTime*4);
        _thrusterOnOff.constantMax = MaxThrusterVelocity * curve.Evaluate(timer/timeToMax);
        _emissionRate.constantMax = _originalEmissionRate.constantMax * curve.Evaluate(timer / timeToMax);
        //Debug.Log("timer: " + timer + " max:" + timeToMax + " frac: " + (timer/timeToMax) + " curve: " +  curve.Evaluate(timer / timeToMax));
        _emissionRate.constantMax = Mathf.Lerp(_emissionRate.constantMax, _originalEmissionRate.constantMax, Time.deltaTime*4);
        _emission.rateOverTime = _emissionRate;
        _velocityOverLifetime.z = _thrusterOnOff;
        audioFraction = Mathf.Lerp(audioFraction, 1, Time.deltaTime);
    }
    public void ThrusterOff() {
        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;
        _thrusterOnOff.constantMax = 0;
        _emissionRate.constantMax = Mathf.Lerp(_emissionRate.constantMax, 0, Time.deltaTime * 10);
        _emission.rateOverTime = _emissionRate;
        audioFraction = Mathf.Lerp(audioFraction, 0, Time.deltaTime);
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
