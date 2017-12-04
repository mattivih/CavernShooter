using UnityEngine;
using System.Collections;

public class TextScroll : MonoBehaviour {

	public float scrollSpeed = 10f;
    private Vector3 _pos;
    private Vector3 _startingPosition = new Vector3(0f, 16f, 16f);

	void Update()
	{
		_pos = transform.position;

		// Vector pointing into the distance
		Vector3 localVectorUp = transform.TransformDirection(0f, 1f, 0f);

		// Move the text object into the distance
		_pos += localVectorUp * scrollSpeed * Time.deltaTime;
		transform.position = _pos;
	}

    void OnEnable()
    {
        transform.position = _startingPosition;
    }
}
