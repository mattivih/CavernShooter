using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : MonoBehaviour {

    ParticleSystem.EmissionModule emission;
    ParticleSystem.MinMaxCurve noEmission = new ParticleSystem.MinMaxCurve();
    public AudioClip clipActivateShield, clipShieldEnd;
    private AudioSource audioActivateShield, audioShieldEnd;

    void Awake() {
        noEmission.constantMax = 0;
        emission = GetComponent<ParticleSystem>().emission;

        audioActivateShield = AddAudio(clipActivateShield, false, false, 1f);
        audioShieldEnd = AddAudio(clipShieldEnd, false, false, 1f);

    }

    void Start() {
        audioActivateShield.Play();
    }

    void Update()
    {
        if (Ship.LocalPlayerInstance.GetComponent<Ship>().Shield <= 0 && GetComponent<PhotonView>().isMine)
            Die();
    }

    public void Die() {
        emission.rateOverTime = noEmission;
        audioShieldEnd.Play();
        float time = Mathf.Max(GetComponent<ParticleSystem>().main.duration, clipShieldEnd.length);
        PhotonNetwork.Destroy(gameObject);
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
