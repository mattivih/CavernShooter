using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MyLobbyManager : NetworkLobbyManager
{
	public static MyLobbyManager Instance;

	[Header("Match Settings")]
	public int MaxMatchesToList = 4;

	[Space]
	[Header("Ships")]
	[Tooltip("1. Untied Fighter 2. Retro Wing 3. Discovery Shuttle 4. U.F.O 5. Tesla Rossa")]
	public GameObject[] ShipPrefabs;

	[Space]
	[Header("UI")]
	public GameObject GameOverPrefab;

	private MyMatchList _matchList;
	private int _currentMatchMaxSize;
	private ulong _currentMatchId;

	//List for player & ship infomation
	private Dictionary<int, int> _playerShipList;

	//List for dead players preserving the dying order
	private List<string> _deadPlayers;

	void Start()
	{
		if (!Instance)
		{
			Instance = this;
		}
		_playerShipList = new Dictionary<int, int>();
		_deadPlayers = new List<string>();
		DontDestroyOnLoad(gameObject);
	}

	// ------------------ Matchmaker calls & callbacks ------------------

	/// <summary>
	/// Starts the Unity Multiplayer service (called Matchmaker)
	/// </summary>
	void StartMatchmaker()
	{
		Instance.StartMatchMaker();
	}

	/// <summary>
	/// Sends request to the server to list matches.
	/// Server's answer is handled by OnMatchList().
	/// </summary>
	public void ListMatches()
	{
		StartMatchmaker();
		Instance.matchMaker.ListMatches(0, 5, "", true, 0, 0, OnMatchList);
	}

	/// <summary>
	/// Scans again for matches in case no matches were found.
	/// </summary>
	public void ScanAgainForMatches()
	{
		_matchList.HideNoMatchesFound();
		_matchList.ShowLoadingIcon();
		Invoke("ListMatches", 2f);
	}

	/// <summary>
	/// Callback function for ListMatches(). If match listing was successfull update UI's match list.
	/// </summary>
	public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
	{

		base.OnMatchList(success, extendedInfo, matches);

		if (!success)
		{
			Debug.LogError("Match listing failed: " + extendedInfo);
		}
		else
		{
			if (!_matchList)
			{
				_matchList = FindObjectOfType<MyMatchList>();
			}
			_matchList.HideLoadingIcon();
			if (matches.Count > 0)
			{
				for (int i = 0; i < matches.Count && i < MaxMatchesToList; i++)
				{
					_matchList.AddMatchToList(matches[i]);
				}

			}
			else
			{
				//No matches found. Show error and scan again.
				if (_matchList)
				{
					_matchList.ShowNoMatchesFound();
					Invoke("ScanAgainForMatches", 1.5f);
				}
			}
		}
	}


	/// <summary>
	/// Sends request to the server to create a match.
	/// Server's answer is handled by OnMatchCreate().
	/// </summary>
	public void CreateMatch(string matchName, uint matchSize)
	{
		StartMatchmaker();
		minPlayers = (int)matchSize; //Requires the same amount of players to be ready in order to start the game as matchSize
		Instance.matchMaker.CreateMatch(matchName, matchSize, true, "", "", "", 0, 0, OnMatchCreate);
	}

	/// <summary>
	/// Callback function for CreateMatch.
	/// </summary>
	public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
	{
		base.OnMatchCreate(success, extendedInfo, matchInfo);

		if (!success)
		{
			Debug.LogError("Creating match failed: " + extendedInfo);

		}
		else
		{
			_currentMatchId = (System.UInt64)matchInfo.networkId;
			Debug.Log("Creating match " + _currentMatchId);
		}
	}

	/// <summary>
	/// Changes the Play Scene to selected map.
	/// </summary>
	/// <param name="name">Scene name</param>
	public void SelectMap(string name)
	{
		playScene = name;
	}

	/// <summary>
	/// Sends request to the server to join a match.
	/// Server's answer is handled by OnMatchJoined().
	/// </summary>
	public void JoinMatch(MatchInfoSnapshot match)
	{
		//Debug.Log("@ JoinMatch");
		Instance.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, OnMatchJoined);
	}

	/// <summary>
	/// Callback function for JoinMatch.
	/// </summary>
	public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
	{
		base.OnMatchJoined(success, extendedInfo, matchInfo);

		if (!success)
		{
			Debug.LogError("Joining match failed: " + extendedInfo);
		}
	}

	// ------------------ Server callbacks ------------------

	/// <summary>
	/// Instantiate a lobby player game object.
	/// </summary>
	/// <returns></returns>
	public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
	{
		base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
		GameObject newPlayer = Instantiate(lobbyPlayerPrefab.gameObject);
		DontDestroyOnLoad(newPlayer);
		if (!_playerShipList.ContainsKey(conn.connectionId))
		{
			_playerShipList.Add(conn.connectionId, 0); //Assign default ship to the player
		}
		newPlayer.GetComponent<MyLobbyPlayer>().ConnectionId = conn.connectionId;
		return newPlayer;
	}

	/// <summary>
	/// When the game starts spawn a corret player prefab
	/// </summary>
	public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
	{
		base.OnLobbyServerCreateGamePlayer(conn, playerControllerId);
		int ship = _playerShipList[conn.connectionId];
		GameObject player = Instantiate(ShipPrefabs[ship], startPositions[conn.connectionId].position, Quaternion.identity);

		//Find player's name
		MyLobbyPlayer[] lobbyPlayers = FindObjectsOfType<MyLobbyPlayer>();
		foreach (MyLobbyPlayer lobbyPlayer in lobbyPlayers)
		{
			if (lobbyPlayer.ConnectionId == conn.connectionId)
			{
				player.name = lobbyPlayer.PlayerName;
			}
		}
		return player;
	}

	/// <summary>
	/// Called when a player dies.
	/// </summary>
	/// <param name="playerName"></param>
	public void OnServerOnPlayerDeath(string playerName)
	{
		_deadPlayers.Add(playerName);

        //TODO: Fetch last alive players name
		if (_deadPlayers.Count == minPlayers - 1)
		{
            MyLobbyPlayer[] lobbyPlayers = FindObjectsOfType<MyLobbyPlayer>();
            foreach (MyLobbyPlayer lobbyPlayer in lobbyPlayers)
            {
                if (!_deadPlayers.Contains(lobbyPlayer.PlayerName)) {
                    _deadPlayers.Add(lobbyPlayer.PlayerName);
                }
            }
                GameManager.Instance.CallEndGame(_deadPlayers.ToArray());
		}
	}

	/// <summary>
	/// Hide the lobby player UI elements the game scene is loaded.
	/// </summary>
	/// <param name="sceneName"></param>
	public override void OnLobbyServerSceneChanged(string sceneName)
	{
		base.OnLobbyServerSceneChanged(sceneName);
		if (sceneName != "1_MainMenu")
		{
			foreach (var player in lobbySlots)
			{
				if (player)
				{
					foreach (Transform child in player.gameObject.transform)
					{
						child.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	// ------------------ Client callbacks ------------------
	/// <summary>
	/// Hide the lobby player UI elements the game scene is loaded.
	/// </summary>
	public override void OnLobbyClientSceneChanged(NetworkConnection conn)
	{
		base.OnLobbyClientSceneChanged(conn);
		MyLobbyPlayer[] players = FindObjectsOfType<MyLobbyPlayer>();

		foreach (var player in players)
		{
			foreach (Transform child in player.transform)
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	// <summary>
	// Called by Lobby Players. Adds player-ship information to the _playersList.
	// </summary>

	public override void OnServerReady(NetworkConnection conn)
	{
		base.OnServerReady(conn);

		//Update UI on self & clients
		foreach (var networkLobbyPlayer in lobbySlots)
		{
			var player = (MyLobbyPlayer)networkLobbyPlayer;
			if (MyUIPlayerList.Instance && player && player.ConnectionId == conn.connectionId)
			{
				MyUIPlayerList.Instance.UpdatePlayerStatus(player);
				player.RpcUpdatePlayerStatus();
			}
		}
	}

	/// <summary>
	/// Adds player-ship information to the _playersList.
	/// </summary>
	public void SaveShipForPlayer(int connectionID, int ship)
	{
		if (_playerShipList.ContainsKey(connectionID))
		{
			_playerShipList[connectionID] = ship;
		}
	}

	//------------------ UI management ------------------

	/// <summary>
	/// Callback function that adds the player information to the UI's player list after the lobby player has been created.
	/// </summary>
	public void AddPlayerToPlayerList(MyLobbyPlayer player)
	{
		MyUIPlayerList playerList = FindObjectOfType<MyUIPlayerList>();
		playerList.AddPlayer(player);
	}
}
