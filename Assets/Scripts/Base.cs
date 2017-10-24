using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour {

    public float HealthRegen, MaxBaseHealth;
    public ParticleSystem BaseExplosion;
    Light[] lights;

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

	/// <summary>
	/// Stops the player movement on the base, and regenerates health for the player.
	/// </summary>
	/// <param name="collision">Collision.</param>
    void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            foreach(Light l in lights) {
                l.color = Color.green;
            }
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
		if (collision.gameObject.GetComponent<ProjectilesBase> ()) {
			TakeDamage (collision.gameObject.GetComponent<ProjectilesBase> ().Damage);
		}
	}

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            foreach(Light l in lights) {
                l.color = Color.red;
            }
        }
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
