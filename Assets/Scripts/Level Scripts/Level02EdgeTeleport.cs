using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level02EdgeTeleport : MonoBehaviour {

    public GameObject Background;

    float _edgeDistance;

    void Start() {
         _edgeDistance = Background.GetComponent<SpriteRenderer>().bounds.extents.y;
    }

    /// <summary>
    /// Teleport the ship to the opposite side of the screen and render edge camera view to the main camera view
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerStay2D(Collider2D collider) {
        //Teleport the ship to the opposite edge of the screen
        if (gameObject.name == "Teleport Collider Upper" && collider.GetComponent<Rigidbody2D>().velocity.y > 0) {
            if (collider.tag == "Player") {
                //Teleport the player the lower part of the screen
                collider.transform.position = new Vector3(collider.transform.position.x, -_edgeDistance, collider.transform.position.z);
            } else if (collider.tag == "Projectile") {
                GameObject newLaser = Instantiate(collider.gameObject, new Vector3(collider.transform.position.x, -_edgeDistance, collider.transform.position.z), collider.transform.rotation);
                newLaser.GetComponent<Rigidbody2D>().velocity = collider.GetComponent<Rigidbody2D>().velocity;
                newLaser.name = collider.name;
                collider.GetComponent<Collider2D>().enabled = false;
                Destroy(collider.gameObject);
            }
        } else if (gameObject.name == "Teleport Collider Lower" && collider.GetComponent<Rigidbody2D>().velocity.y < 0) {
            if (collider.tag == "Player") {
                //Teleport the player the upper part of the screen
                collider.transform.position = new Vector3(collider.transform.position.x, _edgeDistance, collider.transform.position.z);
            } else if (collider.tag == "Projectile") {
                GameObject newLaser = Instantiate(collider.gameObject, new Vector3(collider.transform.position.x, _edgeDistance, collider.transform.position.z), collider.transform.rotation);
                newLaser.GetComponent<Rigidbody2D>().velocity = collider.GetComponent<Rigidbody2D>().velocity;
                newLaser.name = collider.name;
                collider.GetComponent<Collider2D>().enabled = false;
                Destroy(collider.gameObject);
            }
        }

    }
}
