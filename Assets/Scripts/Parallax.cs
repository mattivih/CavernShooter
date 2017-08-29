using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attached to backgrounds, moves the background to same direction as camera(which is attached to player). movement is multiplied by scale
/// </summary>
public class Parallax : MonoBehaviour {

    Vector3 startPos;
    public float Scale=1;

    bool playerFound = false;

    void FixedUpdate() {
        if (GameManager.Instance&&GameManager.Instance.Player) {
            if (!playerFound) {
                startPos = Camera.main.transform.position;
                playerFound = true;
            } else {
                Vector3 diff = startPos - Camera.main.transform.position;
                diff.z = 0;
                startPos = Camera.main.transform.position;
                transform.position += diff * Scale;
            }
        }
    }
}
