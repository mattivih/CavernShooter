using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEnvironmentCollider : Photon.PunBehaviour {

	public float HitVelocityThreshold=50, HitDamageScale=0.1f;
    public AudioClip clipCollide;
    public AudioSource audioCollide;

    void Awake() {
        audioCollide = AddAudio(clipCollide, false, false, 1f);
    }


	/// <summary>
	/// Makes the ship to take damage if it's velocity is greater than HitVelocityThreshold when it hits the walls.
	/// The amout of damage taken can be scaled with HitDamageScale.
	/// </summary>
	/// <param name="collision"></param>
	void OnCollisionEnter2D(Collision2D collision) {
		if (GetComponentInParent<PhotonView>().isMine && collision.relativeVelocity.sqrMagnitude > HitVelocityThreshold) {
            audioCollide.pitch = 0.5f;
            float damage = collision.relativeVelocity.sqrMagnitude * HitDamageScale;
			//Debug.Log("hit wall! damage: " + damage);
			GetComponentInParent<Ship>().TakeDamage(damage, null);
		} else {
            audioCollide.pitch = 1f;
        }
        
        if (GetComponentInParent<PhotonView>().isMine)
            audioCollide.Play();
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
