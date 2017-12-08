using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : MonoBehaviour {

    public Shield parent;

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
    }

    void Update()
    {
        
    }

    public IEnumerator Die() {
        audioActivateShield.Play();
        yield return new WaitForSeconds(parent.shieldDuration);
        emission.rateOverTime = noEmission;
        audioShieldEnd.Play();
        float time = Mathf.Max(GetComponent<ParticleSystem>().main.duration, clipShieldEnd.length);
        GetComponentInParent<Ship>().Shield = false;
        parent.alreadyOn = null;
        yield return new WaitForSeconds(2);
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
