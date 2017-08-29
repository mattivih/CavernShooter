using UnityEngine;

public class LoadingIcon : MonoBehaviour
{

	public float Rotation;

	private RectTransform rect;
	void Start()
	{
		rect = GetComponent<RectTransform>();
	}

	// Update is called once per frame
	void Update()
	{
		rect.Rotate(Vector3.forward, -Rotation * Time.deltaTime);
	}
}
