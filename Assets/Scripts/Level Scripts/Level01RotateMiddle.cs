using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level01RotateMiddle : MonoBehaviour {

    public float Rotation = 8f;

	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward * Rotation * Time.deltaTime, Space.World);
	}
}
