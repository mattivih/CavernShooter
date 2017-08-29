using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level02MirrorCameraController : MonoBehaviour {

    public GameObject Background;
    public GameObject Mirror;

    void Start() {
        float orthoSize = GetComponent<Camera>().orthographicSize;
        Mirror.transform.localScale = new Vector3(orthoSize * 2, orthoSize * 2, 0);
    }

    void Update() {
        if (GameManager.Instance.Player) {

            //Update the edge camera's x & z position to be the same than the main camera's
            transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);


            //Moves the edge camera UP or DOWN depending on player's y position.
            float cameraPosition = Background.GetComponent<SpriteRenderer>().bounds.extents.y;
            if (GameManager.Instance.Player.transform.position.y > 0) {
                transform.position = new Vector3(Camera.main.transform.position.x, -cameraPosition, Camera.main.transform.position.z);
                Mirror.transform.position = new Vector3(Camera.main.transform.position.x, cameraPosition, Mirror.transform.position.z);
            } else {
                transform.position = new Vector3(Camera.main.transform.position.x, cameraPosition, Camera.main.transform.position.z);
                Mirror.transform.position = new Vector3(Camera.main.transform.position.x, -cameraPosition, Mirror.transform.position.z);
            }
        }
    }
}
