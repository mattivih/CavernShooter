﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseTorpedo : ProjectilesBase {

	public float Speed = 10f;
	public float TurningSpeedMultiplier = 1f;
    public float TurningSpeedAngular;
    public float zRotationSpeed;
	public float TorpedoLifetime = 2f;
    float maxAngle = 0.01f;
	public ParticleSystem EmitFire, TorpedoExplosionPrefab;
	public GameObject source = null;

	/// <summary>
	/// Locks on to the enemy player and starts flying torwards it at a given speed and turning speed.
	/// </summary>
	void Update()
	{
		GameObject[] targetObjects = GameObject.FindGameObjectsWithTag("Enemy");
		if (targetObjects.Length==0) {
			Vector2 moveDirection = GetComponent<Rigidbody2D>().velocity;
			float angle = 0f;
			if (moveDirection != Vector2.zero) {
				angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
			}
			Quaternion newRot = Quaternion.AngleAxis(angle, Vector3.forward);
			if (transform.rotation != newRot) {
				transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.deltaTime * TurningSpeedMultiplier*0.3f);
			}

		} else {
			float closest = float.MaxValue;
			GameObject closestTarget=null;
			for(int i=0;i<targetObjects.Length;i++) {
				float dist = Vector3.Distance(targetObjects[i].transform.position, transform.position);
				if (dist < closest) {
					closest = dist;
					closestTarget = targetObjects[i];
				}
			}
			Vector3 Target = closestTarget.transform.position;
			Vector3 Distance = Target - transform.position;

			Vector2 lookDir = transform.up;

			Vector3 cross = Vector3.Cross(Distance, lookDir);
			// actually get the sign (either 1 or -1)
			float sign = Mathf.Sign(cross.z);

			// the angle, ranging from 0 to 180 degrees
			float angle = Vector2.Angle(Distance, lookDir);

			float turn = TurningSpeedMultiplier * Mathf.Sqrt(Mathf.Abs(angle)/ TurningSpeedAngular);
			// apply the sign to get angles ranging from -180 to 0 to +180 degrees
			angle *= sign;
			// apply torque in the opposite direction to decrease angle
			if (Mathf.Abs(angle) > maxAngle) {
				GetComponent<Rigidbody2D>().AddTorque(-sign * turn);
			}
		}

		Vector3 meshRot = GetComponentInChildren<MeshRenderer>().gameObject.transform.eulerAngles;
		meshRot.z += Time.deltaTime * zRotationSpeed;
		GetComponentInChildren<MeshRenderer>().gameObject.transform.eulerAngles = meshRot;

		GetComponent<Rigidbody2D>().AddForce(transform.up * Speed, ForceMode2D.Force);
	}

    /// <summary>
    /// Torpedo detonates when it comes close enough to an enemy player or terrain.
    /// </summary>
    /// <param name="collider">Collider.</param>
    void OnCollisionEnter2D(Collision2D collision) {
		ParticleSystem t = Instantiate(TorpedoExplosionPrefab, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
		t.transform.position += t.transform.up * 0.5f;
		Destroy (t.gameObject, TorpedoLifetime );
        Destroy(EmitFire.gameObject, TorpedoLifetime);
        // This splits the particle off so it doesn't get deleted with the parent
        EmitFire.transform.parent = null;
		// this stops the particle from creating more bits
		EmitFire.emissionRate = 0;
        Destroy(EmitFire, EmitFire.main.duration);
		if (collision.collider.tag == "Enemy") {
			collision.gameObject.GetComponent<Ship> ().TakeDamage (Damage, source);
		}
		Destroy (gameObject);
	}
}