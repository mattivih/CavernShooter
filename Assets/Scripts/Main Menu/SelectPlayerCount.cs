using UnityEngine;

public class SelectPlayerCount : MonoBehaviour
{

	public GameObject ShipSprite;
	public GameObject[] Sprites;

	void Start()
	{
		Vector3 _distance = new Vector3(0, 0, 0);
		ShipSprite = Sprites[0];
		for (int i = 0; i < Sprites.Length; i++)
		{
			if (i % 2 == 0)
			{
				Instantiate(ShipSprite, transform.position + _distance, Quaternion.identity, transform);
				_distance += new Vector3(1, 0, 0);
			}
			else
			{
				Instantiate(ShipSprite, transform.position + _distance, Quaternion.Euler(0, 0, 180), transform);
				_distance += new Vector3(1, 0, 0);
			}
		}
	}
}
