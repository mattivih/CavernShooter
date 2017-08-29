using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	public void DestroyDelayed(float time) {
        Destroy(gameObject, time);
    }
}
