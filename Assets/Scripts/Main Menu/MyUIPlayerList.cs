using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// UI script: handles the Players Joined & Connection status-list.
/// Note: this handles only the UI list, the list of lobby player objects and their status is handled in MyLobbyManager script.
/// </summary>
public class MyUIPlayerList : MonoBehaviour
{
	//Public variables
	public static MyUIPlayerList Instance = null;
	public GameObject PlayerListEntryPrefab;

	//Private variables
	private List<Transform> _positions;
	private int _playerCount = 0;

	/// <summary>
	/// Make the MyPlayerList instance a singleton.
	/// </summary>
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	/// <summary>
	/// Create a list for the players that join the match's lobby.
	/// Save the Y pos of the first item in the list (the default y pos of the prefab)
	///  so the consecutive list items can be added below it.
	/// </summary>
	void Start()
	{
		_positions = GetComponentsInChildren<Transform>(true).ToList();

		//Delete parent object from the list
		Transform parent = null;
		foreach (var pos in _positions)
		{
			if (pos.gameObject.GetComponent<MyUIPlayerList>())
			{
				parent = pos;
			}
		}
		_positions.Remove(parent);
	}

	/// <summary>
	///Adds player to the Players Joined & Connection status-list
	/// </summary>
	public void AddPlayer(MyLobbyPlayer player)
	{
		GameObject playerListEntry = Instantiate(PlayerListEntryPrefab);
		playerListEntry.GetComponent<MyPlayerListEntry>().FillPlayerListEntry(player);

		//Add the new player info below the previous entry.
		if (_playerCount < _positions.Count)
		{
			playerListEntry.transform.SetParent(_positions[_playerCount], false);
			_playerCount++;
		}
	}

	/// <summary>
	/// Clears the whole list
	/// </summary>
	//private void ClearList()
	//{
	//	_playerCount = 0;
	//	MyPlayerListEntry[] listEntries = GetComponentsInChildren<MyPlayerListEntry>();
	//	foreach (var entry in listEntries)
	//	{
	//		Destroy(entry.gameObject);
	//	}
	//}

	/// <summary>
	/// Updates player's status in Players Joined & Connection status-list
	/// </summary>
	public void UpdatePlayerStatus(MyLobbyPlayer player)
	{
		MyPlayerListEntry[] listEntries = GetComponentsInChildren<MyPlayerListEntry>();
		foreach (var entry in listEntries)
		{
			if (entry.Player.netId == player.netId)
			{
				if (player.readyToBegin)
				{
					entry.PlayerReady();

				}
				else
				{
					entry.PlayerNotReady();
				}
			}
		}
	}

	/// <summary>
	/// Updates player's name in Players Joined & Connection status-list
	/// </summary>
	public void UpdatePlayerName(MyLobbyPlayer player)
	{
		MyPlayerListEntry[] listEntries = GetComponentsInChildren<MyPlayerListEntry>();

		foreach (var entry in listEntries)
		{
			if (entry.Player.netId == player.netId)
			{
				entry.SetName(player.PlayerName);
			}
		}
	}

	/// <summary>
	/// Updates player's name in Players Joined & Connection status-list
	/// </summary>
	//public void DeletePlayer(MyLobbyPlayer player)
	//{
	//	_playerCount--;
	//	if (_playerCount <= 0)
	//	{
	//		ClearList();
	//	}
	//	else
	//	{
	//		MyPlayerListEntry[] listEntries = GetComponentsInChildren<MyPlayerListEntry>();
	//		foreach (var entry in listEntries)
	//		{
	//			if (entry.Player.ConnectionId == player.ConnectionId)
	//			{
	//				Destroy(entry.gameObject);
	//			}
	//		}
	//	}
	//}

	/// <summary>
	/// Returns the current count of the players in the match lobby.
	/// </summary>
	/// <returns>Count of the players in the match lobby</returns>
	//TODO: Remove when user can submit their own name
	public int GetPlayerCount()
	{
		return transform.childCount + 1;
	}

	//TODO: RemovePlayer()
}
