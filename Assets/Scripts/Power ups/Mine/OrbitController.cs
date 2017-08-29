using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour {

		void OnTriggerEnter2D(Collider2D collider) {
		if(collider.tag == "Mine") {
			collider.transform.parent.parent = this.transform;
		}
	}
}
