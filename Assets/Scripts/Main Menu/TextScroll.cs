using UnityEngine;
using System.Collections;

public class TextScroll : MonoBehaviour {

	public float scrollSpeed = 20f;

	void Update()
	{
		Vector3 pos = transform.position;

		// Vector pointing into the distance
		Vector3 localVectorUp = transform.TransformDirection(0f, 1f, 0f);

		// Move the text object into the distance
		pos += localVectorUp * scrollSpeed * Time.deltaTime;
		transform.position = pos;
	}

}
