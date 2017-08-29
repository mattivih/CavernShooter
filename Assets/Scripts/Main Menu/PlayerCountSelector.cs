﻿using UnityEngine;
using UnityEngine.UI;

public class PlayerCountSelector : MonoBehaviour
{
	public static int PlayersSelected = 2;
	public Sprite Active, Unactive, ActiveHover, UnactiveHover;
	private Image _image;

	void Start()
	{
		_image = GetComponent<Image>();
	}

	//Start hoovering
	void OnMouseEnter()
	{
		if (_image.sprite.name == "BTN-ship-UNACTIVE-01")
		{
			ChangeSprites(UnactiveHover);
		}
		else if (_image.sprite.name == "BTN-ship-ACTIVE-01")
		{
			ChangeSprites(ActiveHover);
		}
	}

	//End hoovering
	void OnMouseExit()
	{
		if (_image.sprite.name == "BTN-ship-UNACTIVE-HOVER-01")
		{
			ChangeSprites(Unactive);
		}
		else if (_image.sprite.name == "BTN-ship-ACTIVE-HOVER-01")
		{
			ChangeSprites(Active);
		}
	}

	//Select/unselecet
	void OnMouseDown()
	{
		//Select
		if (_image.sprite.name == "BTN-ship-UNACTIVE-HOVER-01" ||
		_image.sprite.name == "BTN-ship-UNACTIVE-01")
		{
			ChangeSprites(Active);
			PlayersSelected++;
		}
		else if (_image.sprite.name == "BTN-ship-ACTIVE-HOVER-01" ||
	   _image.sprite.name == "BTN-ship-ACTIVE-01")
		{
			//Unselect
			ChangeSprites(Unactive);
			PlayersSelected--;
		}
	}

	public void ChangeSprites(Sprite sprite)
	{
		_image.sprite = sprite;
	}

}
