﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : Photon.PunBehaviour {

    public SpriteRenderer spriteRenderer;
    public float HealthRegen, MaxBaseHealth;
    public ParticleSystem BaseExplosion;
    public ParticleSystem BaseSmoke60;
    public ParticleSystem BaseSmoke301;
    public ParticleSystem BaseSmoke302;
    private Color _color;
    //private bool _isCollidingPlayer = false;
    //private bool _isCollidingEnemy = false;
    

    //[SyncVar]
    //TODO: Refactor with [Server]
    public float BaseHealth;
    private bool _lightToggle = false;
    float LightTimer = 0.1f;

    /// <summary>
	/// Sets base health to max health.
    /// </summary>
    void Start() {
		BaseHealth = MaxBaseHealth;
        GetComponent<PhotonView>().RPC("LightsOff", PhotonTargets.All, null);
    }


    /// <summary>
    /// Check if base is to be destroyed.
    /// </summary>
    void Update() {
        if (BaseHealth <= 0)
        {
            DestroyBase();
        }

        if (BaseHealth > 0 && BaseHealth <= (MaxBaseHealth * 0.3f))
        {
            if (!BaseSmoke60.isPlaying)
                BaseSmoke60.Play();
            if (!BaseSmoke301.isPlaying)
                BaseSmoke301.Play();
            if (!BaseSmoke302.isPlaying)
                BaseSmoke302.Play();
        }

        if (BaseHealth > (MaxBaseHealth * 0.3f) && BaseHealth <= (MaxBaseHealth * 0.6f))
        {
            BaseSmoke301.Stop();
            BaseSmoke302.Stop();
            if (!BaseSmoke60.isPlaying)
                BaseSmoke60.Play();
        }

        if (BaseHealth > (MaxBaseHealth * 0.6f))
        {
            BaseSmoke301.Stop();
            BaseSmoke302.Stop();
            BaseSmoke60.Stop();
        }

        if (_lightToggle)
        {
            LightTimer -= Time.deltaTime;
            if (LightTimer <= 0)
            {
                _lightToggle = false;
                LightTimer = 0.1f;
            }
        }
    }

    [PunRPC]
    public void LightsOn()
    {
        Color color = new Color(0, 255, 0);
        var lights = GetComponentsInChildren<Light>();
        foreach (var light in lights)
        {
            light.color = color;
            light.intensity = 0.025f;
        }
    }

    [PunRPC]
    public void LightsOff()
    {
        Color color = new Color(255, 0, 0);
        var lights = GetComponentsInChildren<Light>();
        foreach (var light in lights)
        {
            light.color = color;
            light.intensity = 0.025f;
        }
    }

    [PunRPC]
    public void BlinkLights()
    {
        var lights = GetComponentsInChildren<Light>();
        foreach (var light in lights)
        {
            light.intensity = 0.05f;
        }
        StartCoroutine(ResetLightIntensity());
    }

    public IEnumerator ResetLightIntensity()
    {
        yield return new WaitForSeconds(0.01f);
        var lights = GetComponentsInChildren<Light>();
        foreach (var light in lights)
        {
            light.intensity = 0.025f;
        }
    }

    /// <summary>
    /// Stops the player movement on the base, and regenerates health for the player.
    /// </summary>
    /// <param name="collision">Collision.</param>
    void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Ship>()) {

            collision.gameObject.GetComponent<Ship>().IncreaseHealth(HealthRegen * Time.deltaTime);
          
            if (!Input.anyKey) {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                collision.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Ship>())
        {
            if (!_lightToggle)
                GetComponent<PhotonView>().RPC("LightsOff", PhotonTargets.All);
        }
    }
    
    /// <summary>
    /// Call for Base to take damage if hit by a projectile.
    /// </summary>
    /// <param name="collision">Collision.</param>
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Ship>())
        { 
            if (!_lightToggle)
            {
                GetComponent<PhotonView>().RPC("LightsOn", PhotonTargets.All);
            }
        }
        if (collision.gameObject.GetComponent<ProjectilesBase>()) {
            TakeDamage(collision.gameObject.GetComponent<ProjectilesBase>().Damage);
            //GetComponent<PhotonView>().RPC("BlinkLights", PhotonTargets.All, null);
           // _lightToggle = true;
        }
    }

	/// <summary>
	/// Call for Base to take damage if hit by a projectile.
	/// </summary>
	/// <param name="collider">Collider.</param>
	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.GetComponent<ProjectilesBase>()) {
			TakeDamage (collider.gameObject.GetComponent<ProjectilesBase> ().Damage);
            //GetComponent<PhotonView>().RPC("BlinkLights", PhotonTargets.All, null);
            // _lightToggle = true;
        }
    }

	/// <summary>
	/// Damage the base.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public void TakeDamage(float damage) {
		BaseHealth = BaseHealth - damage;
        GetComponent<PhotonView>().RPC("BlinkLights", PhotonTargets.All, null);
        _lightToggle = true;
    }

    void DestroyBase()
    {     
        if (PhotonNetwork.isMasterClient)
        {
            GameObject explosion = PhotonNetwork.Instantiate("BaseExplosion", gameObject.transform.position, gameObject.transform.rotation, 0);
            photonView.RPC("BaseExplosionSound", PhotonTargets.All, explosion.GetPhotonView().viewID);
            PhotonNetwork.Instantiate("BaseExplosion", gameObject.transform.position + new Vector3(1, 0, 0), gameObject.transform.rotation, 0);
            PhotonNetwork.Instantiate("BaseExplosion", gameObject.transform.position + new Vector3(-1, 0, 0), gameObject.transform.rotation, 0);
            PhotonNetwork.Destroy(gameObject);
        } 
    }

    [PunRPC]
    public void BaseExplosionSound(int viewid)
    {
        PhotonView.Find(viewid).gameObject.GetComponent<AudioSource>().Play();
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
            stream.SendNext(BaseHealth);
        }
        else
        {
            BaseHealth = (float)stream.ReceiveNext();
        }
    }
}
