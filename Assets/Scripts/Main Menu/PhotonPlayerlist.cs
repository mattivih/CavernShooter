﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// UI script: handles the Players Joined & Connection status-list.
/// Note: this handles only the UI list, the list of lobby player objects and their status is handled in MyLobbyManager script.
/// </summary>
public class PhotonPlayerlist : MonoBehaviour
{
    //Public variables
    public static PhotonPlayerlist Instance = null;
    public GameObject PlayerListEntryPrefab;

    //Private variables
    private List<Transform> _positions;
    private int _playerCount = 0;

    /// <summary>
    /// Make the PlayerList instance a singleton.
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
        //Get the Transforms of the parent and child objects
        _positions = GetComponentsInChildren<Transform>(true).ToList();

        //Delete parent object from the list
        Transform parent = null;
        foreach (var pos in _positions)
        {
            if (pos.gameObject.GetComponent<PhotonPlayerlist>())
            {
                parent = pos;
            }
        }
        _positions.Remove(parent);
    }

    public void Update()
    {
        if (PhotonNetwork.playerList.Length != _playerCount && PhotonNetwork.inRoom) {
            ClearList();
            foreach (var player in PhotonNetwork.playerList) {
                AddPlayer(player);
            }
        }
    }


    /// <summary>
    ///Adds player to the Players Joined & Connection status-list
    /// </summary>
    public void AddPlayer(PhotonPlayer player)
    {
        GameObject playerlistEntry = Instantiate(PlayerListEntryPrefab);
        playerlistEntry.GetComponent<PhotonPlayerlistEntry>().FillPlayerListEntry(player);

        //Add the new player info below the previous entry.
        if (_playerCount < _positions.Count)
        {
            playerlistEntry.transform.SetParent(_positions[_playerCount], false);
            _playerCount++;
        }
    }

    public PhotonPlayerlistEntry[] GetPlayerlist()
    {
        return GetComponentsInChildren<PhotonPlayerlistEntry>();
    }


    /// <summary>
    /// Clears the whole list
    /// </summary>
    private void ClearList()
    {
        _playerCount = 0;
        PhotonPlayerlistEntry[] playerlist = GetPlayerlist();
        foreach (var entry in playerlist)
        {
            Destroy(entry.gameObject);
        }
    }

    public void DeletePlayer(PhotonPlayer player)
    {
        _playerCount--;
        if (_playerCount <= 0)
        {
            ClearList();
        }
        else
        {
            PhotonPlayerlistEntry[] playerlist = GetPlayerlist();
            foreach (var entry in playerlist)
            {
                if (entry.Player.ID == player.ID)
                {
                    Destroy(entry.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Updates player's status in Players Joined & Connection status-list
    /// </summary>
    public void UpdatePlayerStatus(int playerID, bool isReady)
    {
        //PhotonPlayerlistEntry[] playerlist = GetPlayerlist();
        //    foreach (var entry in playerlist)
        //    {
        //        if (entry.Player.ID == playerID)
        //        {
        //            if (isReady)
        //            {
        //                entry.PlayerReady();

        //            }
        //            else
        //            {
        //                entry.PlayerNotReady();
        //            }
        //        }
        //    }
        //Force UI update
        ClearList();
        }
}


/// <summary>
/// Updates player's name in Players Joined & Connection status-list
/// </summary>
//public void UpdatePlayerName(MyLobbyPlayer player)
//{
//	PhotonPlayerlistEntry[] listEntries = GetComponentsInChildren<PhotonPlayerlistEntry>();

//	foreach (var entry in listEntries)
//	{
//		if (entry.Player.netId == player.netId)
//		{
//			entry.SetName(player.PlayerName);
//		}
//	}
//}

/// <summary>
/// Updates player's name in Players Joined & Connection status-list
/// </summary>

