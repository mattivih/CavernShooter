using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attached to ships. detects if ship is within lightning's polygon collider trigger and reverses turning for DistortTime seconds. also makes lightning visually target the ship
/// </summary>

public class ShipLightningTrigger : MonoBehaviour {

    public float DistortTime = 10f;
    public AudioClip HitByDistortion;
    float distortTimer = 0;
    bool distorted = false;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = AddAudio(HitByDistortion, false, false, 0.5f);
        _audioSource.spatialBlend = 1f;
        _audioSource.dopplerLevel = 0.1f;
        _audioSource.maxDistance = 15f;
    }

    void Update() {
        if (distorted) {
            distortTimer += Time.deltaTime;
            if (distortTimer > DistortTime) {
                GetComponent<Ship>().Rotation *= -1;
                distortTimer = 0f;
                distorted = false;
                _audioSource.Stop();
            }
        }
    }


    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Lightning") {
            if (collider.GetComponent<LightningController>().TryAddTarget(transform)) {
                if (!distorted) {
                    GetComponent<Ship>().Rotation *= -1;
                }
                distortTimer = 0f;
                distorted = true;
                if (_audioSource.isPlaying)
                    _audioSource.Stop();
                _audioSource.Play();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.tag == "Lightning") {
            collider.GetComponent<LightningController>().targets.Remove(transform);
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
}
