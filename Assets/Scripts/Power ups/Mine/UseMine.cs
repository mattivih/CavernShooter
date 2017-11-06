using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseMine : ProjectilesBase {

	private GameObject _base;
	private GameObject _rotating;
	public float MineLifetime = 3f;
	public GameObject source = null;
	public ParticleSystem MineExplosionPrefab;

	/// <summary>
	/// Blows up mine if enemy ship comes too close to the mine, and gives enemy ship damage. Also checks if there is a base within the mines circle of damage.
	/// </summary>
	/// <param name="collider">Collider.</param>
	void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.GetComponent<Ship>() && collider.gameObject != source) {         
            ParticleSystem m = Instantiate(MineExplosionPrefab, transform.position, transform.rotation);
			m.transform.position += m.transform.forward * -4f;
			Destroy (m, MineLifetime);
			collider.GetComponent<Ship>().TakeDamage(Damage, source);
			Destroy (gameObject);
			if (_base)
				_base.GetComponent<Base>().TakeDamage(Damage);
		} else if (collider.tag == "Base") {
			_base = collider.gameObject;
		}
	}
}
