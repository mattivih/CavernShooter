using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Wrap : MonoBehaviour {

    public GameObject Background;

    private string _levelName;

    void Start() {
        Background = GameManager.Instance.Background;
        _levelName = SceneManager.GetActiveScene().name;

    }


    void FixedUpdate() {
        if (_levelName == "3_Limbo") {
            float backgroundYSize = Background.GetComponent<SpriteRenderer>().bounds.extents.y;
            float yVelocity = GetComponent<Rigidbody2D>().velocity.y;


            if ((transform.position.y > backgroundYSize && yVelocity > 0) || (transform.position.y < -backgroundYSize && yVelocity < 0)) {
                if (tag == "Projectile") {
                    GameObject newLaser = Instantiate(gameObject, new Vector3(transform.position.x, -transform.position.y, transform.position.z), transform.rotation);
                    newLaser.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
                    newLaser.name = name;
                    GetComponent<Collider2D>().enabled = false;
                    GetComponent<Renderer>().enabled = false;
                    tag = "ProjectileOld";
                    Destroy(gameObject, 1);
                } else if (tag != "ProjectileOld") {
                    transform.position = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
                    if (tag == "Player") {
                        if (yVelocity > 0) {
                            //Debug.Log("Up");
                            GameManager.SpriteManager.MoveSpritesDown();
                        } else {
                            //Debug.Log("Down");
                            GameManager.SpriteManager.MoveSpritesUp();
                        }
                    }
                }
            }
        }



    }
}
