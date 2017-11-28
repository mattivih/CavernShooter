using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour {

	void Update ()
    {
        gameObject.GetComponent<Light>().intensity += 1.0f * Time.deltaTime;
		
	}
}
