using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHealthBar : MonoBehaviour {

	public RectTransform HealthBar;
	public float HealthBarWidth;
	private Ship _ship;

	// Use this for initialization
	void Start () {
		_ship = GameObject.FindObjectOfType<Ship> ();
		HealthBar = GameObject.Find ("Foreground").GetComponent<RectTransform> ();
		HealthBarWidth = HealthBar.sizeDelta.x;
	}
	
	// Update is called once per frame
	void Update () {
		HealthBar.sizeDelta = new Vector2(_ship.Health / _ship.MaxHealth * HealthBarWidth, HealthBar.sizeDelta.y);
	}
}
