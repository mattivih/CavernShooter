﻿using UnityEngine;
using UnityEngine.UI;

public class PhotonMapManager : MonoBehaviour
{

	public GameObject[] Maps;
	private Text _mapName;
	private int _currentMap = 0;

	void OnEnable()
	{
		_mapName = GetComponent<Text>();
        ChangeMap(0);
	}

	public void OnClickNextMap()
	{
		ChangeMap(+1);
	}

	public void OnClickPrevMap()
	{
        ChangeMap(-1);
	}

	private void ChangeMap(int direction)
	{
		// Aktivoi/deaktivoi karttakuvaa taulukosta suunnan mukaisesti
		Maps[_currentMap].SetActive(false);
		_currentMap += direction;
		if (_currentMap > Maps.Length - 1)
		{
			_currentMap = 0;
		}
		else if (_currentMap < 0)
		{
			_currentMap = Maps.Length - 1;
		}
		Maps[_currentMap].SetActive(true);

		// Vaihtaa aktiivisena olevan aluksen nimen otsikkoon
		_mapName.text = Maps[_currentMap].name.ToUpper();

		//Informs the lobby manager which scene is chosen
		if (Maps[_currentMap].name == "Nort")
		{
			PhotonLobbyManager.Instance.SelectMap("2_Nort");
		}
		else if (Maps[_currentMap].name == "Limbo")
		{
			PhotonLobbyManager.Instance.SelectMap("3_Limbo");
		}
	}

}
