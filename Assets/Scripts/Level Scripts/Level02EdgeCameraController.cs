using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level02EdgeCameraController : MonoBehaviour {

    public GameObject Background;

    private Camera _edgeCamera;
    private float _originalOrthoSize;

    void Start() {
        _edgeCamera = GetComponent<Camera>();
        _originalOrthoSize = Camera.main.orthographicSize;
    }

    void Update() {
        if (GameManager.Instance.Player) {

            //Update the edge camera's x & z position to be the same than the main camera's
            transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);

            //If player is on the upper part of the screen move edge camera to bottom
            //else move edge camera to top
            float backgroundYSize = Background.GetComponent<SpriteRenderer>().bounds.extents.y;
            float edgeCameraSize = GetComponent<Camera>().orthographicSize;
            float edgeDistance = backgroundYSize + _originalOrthoSize;
            float playerToEdgeDistance = 0;

            //Moves the edge camera UP or DOWN depending on player's y position.
            if (GameManager.Instance.Player.transform.position.y < 0) {
                transform.position = new Vector3(Camera.main.transform.position.x, edgeDistance-_originalOrthoSize*2, Camera.main.transform.position.z);
                playerToEdgeDistance = backgroundYSize + GameManager.Instance.Player.transform.position.y;
            } else {
                transform.position = new Vector3(Camera.main.transform.position.x, -edgeDistance+_originalOrthoSize*2, Camera.main.transform.position.z);
                playerToEdgeDistance = backgroundYSize - GameManager.Instance.Player.transform.position.y;
            }

            //Debug.Log("edge dist: " + playerToEdgeDistance + ", cam pos:" +Camera.main.transform.position + ", player pos:" + GameManager.Instance.Player.transform.position);

            //Starts rendering the edge camera when the player is near the upper or lower edge.
            if (playerToEdgeDistance < _originalOrthoSize && playerToEdgeDistance > 0) {

                float mainCameraViewportHeight = ((playerToEdgeDistance / _originalOrthoSize) * (_originalOrthoSize / 100))/2+0.5f; //Scales mainCameraViewport height from 100% to 50%, hence 0.5f.
                float edgeCameraViewportHeight = 1 - mainCameraViewportHeight;

                if (GameManager.Instance.Player.transform.position.y > 0) { //EDGE down, MAIN up
                    //Assigns the new viewport dimensions and scales the ortographic size accordingly.
                    _edgeCamera.rect = new Rect(new Vector2(0, mainCameraViewportHeight), new Vector2(1, edgeCameraViewportHeight));
                    Camera.main.rect = new Rect(new Vector2(0, 0), new Vector2(1, mainCameraViewportHeight));
                    _edgeCamera.orthographicSize = edgeCameraViewportHeight * _originalOrthoSize;
                    Camera.main.orthographicSize = mainCameraViewportHeight * _originalOrthoSize;


                    transform.position = new Vector3(Camera.main.transform.position.x, -backgroundYSize + (_originalOrthoSize/2-playerToEdgeDistance/2), Camera.main.transform.position.z);

                    //Calculates how much the main camera needs to move so that the player stays in the middle of the screen
                    float yoffset = -((_originalOrthoSize / 2) * (1-(playerToEdgeDistance / _originalOrthoSize)));
                    Camera.main.GetComponent<CameraController>().YOffset = yoffset;
                    //Debug.Log("off: " + yoffset + ", ort: " + _originalOrthoSize + " dist: " + playerToEdgeDistance + ", div: " + (1 - (playerToEdgeDistance / _originalOrthoSize)));
                } else { //EDGE up, MAIN down
                    //Assigns the new viewport dimensions and scales the ortographic size accordingly.
                    _edgeCamera.rect = new Rect(new Vector2(0, 0), new Vector2(1, edgeCameraViewportHeight));
                    Camera.main.rect = new Rect(new Vector2(0, edgeCameraViewportHeight), new Vector2(1, mainCameraViewportHeight));
                    _edgeCamera.orthographicSize = edgeCameraViewportHeight * _originalOrthoSize;
                    Camera.main.orthographicSize = mainCameraViewportHeight * _originalOrthoSize;

                    transform.position = new Vector3(Camera.main.transform.position.x, backgroundYSize - (_originalOrthoSize/2-playerToEdgeDistance/2), Camera.main.transform.position.z);

                    //Calculates how much the main camera needs to move so that the player stays in the middle of the screen
                    float yoffset = ((_originalOrthoSize / 2) * (1-(playerToEdgeDistance / _originalOrthoSize)));
                    Camera.main.GetComponent<CameraController>().YOffset = yoffset;
                }
            } else {
                //Resets sizes of the cameras to their originals
                _edgeCamera.rect = new Rect(Vector2.zero, Vector2.one);
                Camera.main.rect = new Rect(Vector2.zero, Vector2.one);
                _edgeCamera.orthographicSize = _originalOrthoSize;
                Camera.main.orthographicSize = _originalOrthoSize;
                Camera.main.GetComponent<CameraController>().YOffset = 0;

            }
        }
    }
}
