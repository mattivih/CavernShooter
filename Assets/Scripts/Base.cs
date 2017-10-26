﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : Photon.PunBehaviour {

    public SpriteRenderer spriteRenderer;
    public float HealthRegen, MaxBaseHealth;
    public ParticleSystem BaseExplosion;
    Light[] lights;
    private Color _color;
    private bool _isCollidingPlayer = false;
    private bool _isCollidingEnemy = false;


    //[SyncVar]
    //TODO: Refactor with [Server]
    public float BaseHealth;

    /// <summary>
	/// Sets base health to max health.
    /// </summary>
    void Start() {
		BaseHealth = MaxBaseHealth;
        lights = GetComponentsInChildren<Light>();
    }

    /// <summary>
    /// Check if base is to be destroyed.
    /// </summary>
    void Update() {
        if (BaseHealth <= 0)
        {
            DestroyBase();
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
            light.intensity = 0.05f;
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
            light.intensity = 0.05f;
        }


    }

        /// <summary>
        /// Stops the player movement on the base, and regenerates health for the player.
        /// </summary>
        /// <param name="collision">Collision.</param>
        void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy") {
            if(collision.gameObject.tag == "Player")
                _isCollidingPlayer = true;
            if(collision.gameObject.tag == "Enemy")
                _isCollidingEnemy = true;

            //if (isServer) {
            //    collision.gameObject.GetComponent<Ship>().RpcIncreaseHealth(HealthRegen * Time.deltaTime);
            //}
            collision.gameObject.GetComponent<Ship>().IncreaseHealth(HealthRegen * Time.deltaTime);
          
            if (!Input.anyKey) {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                collision.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
            }

        }
    }


    /// <summary>
    /// Call for Base to take damage if hit by a projectile.
    /// </summary>
    /// <param name="collision">Collision.</param>
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        {
            photonView.RPC("LightsOn", PhotonTargets.AllBuffered, null);
     
        }
                        
        if (collision.gameObject.GetComponent<ProjectilesBase> ()) {
			TakeDamage (collision.gameObject.GetComponent<ProjectilesBase> ().Damage);
		}
	}

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player")
            _isCollidingPlayer = false;
        if (collision.gameObject.tag == "Enemy")
            _isCollidingEnemy = false;
        if(!_isCollidingPlayer && !_isCollidingEnemy)
            photonView.RPC("LightsOff", PhotonTargets.AllBuffered, null);
    }

	/// <summary>
	/// Call for Base to take damage if hit by a projectile.
	/// </summary>
	/// <param name="collider">Collider.</param>
	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.GetComponent<ProjectilesBase> ()) {
			TakeDamage (collider.gameObject.GetComponent<ProjectilesBase> ().Damage);
		}
	}

	/// <summary>
	/// Damage the base.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public void TakeDamage(float damage) {
		BaseHealth = BaseHealth - damage;
	}

    void DestroyBase()
    {
        Instantiate(BaseExplosion, gameObject.transform.position, gameObject.transform.rotation);
        Instantiate(BaseExplosion, gameObject.transform.position + new Vector3(1, 0, 0), gameObject.transform.rotation);
        Instantiate(BaseExplosion, gameObject.transform.position + new Vector3(-1, 0, 0), gameObject.transform.rotation);
        Destroy(gameObject);
    }
}
