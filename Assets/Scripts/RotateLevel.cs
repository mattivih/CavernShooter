using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLevel : MonoBehaviour {

    public float RotateSpeed = 8f;

	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward * RotateSpeed * Time.deltaTime, Space.World);
	}
}
