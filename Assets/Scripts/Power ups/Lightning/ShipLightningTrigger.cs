using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attached to ships. detects if ship is within lightning's polygon collider trigger and reverses turning for DistortTime seconds. also makes lightning visually target the ship
/// </summary>

public class ShipLightningTrigger : MonoBehaviour {

    public float DistortTime = 10f;
    float distortTimer = 0;
    bool distorted = false;

    void Update() {
        if (distorted) {
            distortTimer += Time.deltaTime;
            if (distortTimer > DistortTime) {
                GetComponent<Ship>().Rotation *= -1;
                distortTimer = 0f;
                distorted = false;
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
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.tag == "Lightning") {
            collider.GetComponent<LightningController>().targets.Remove(transform);
        }
    }
}
