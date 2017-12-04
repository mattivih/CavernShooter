using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PhotonLobbyManager : Photon.PunBehaviour
{
    #region Public Variables

    public static PhotonLobbyManager Instance;

    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

    #endregion


    #region Private Variables
    private string _selectedMap = "1_Nort"; //Default level

    /// <summary>
    /// This client's game version number. Users are separated from each other by game version (which allows you to make breaking changes).
    /// </summary>
    string _gameVersion = "1";


        #endregion


    #region MonoBehaviour CallBacks

        void Awake()
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.logLevel = Loglevel;
        }

        void Start()
        {
        if (!Instance)
        {
            Instance = this;
        }
        //sourceDontDestroyOnLoad(gameObject);
        PhotonNetwork.ConnectUsingSettings(_gameVersion);
    }


    #endregion

    #region Match & Lobby methods

    public void CreateMatch(string name, int playerCount)
    {

        if (string.IsNullOrEmpty(name))
        {
            name = "Match";
        }

        PlayerNotReady();

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = (byte)playerCount };
        Hashtable matchProperties = new Hashtable() { { "MatchName", name }, { "SelectedMap", _selectedMap } };
        roomOptions.CustomRoomPropertiesForLobby = new[] {"MatchName", "SelectedMap"};
        roomOptions.CustomRoomProperties = matchProperties;

        if (PhotonNetwork.connected)
        {
            PhotonNetwork.CreateRoom(null, roomOptions , null);
        }
    }

    /// <summary>
    /// Joins the lobby - start receiving matchlist updates.
    /// </summary>
    public void JoinLobby() {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// Leaves the lobby - stop receiving matchlist updates.
    /// </summary>
    public void ExitLobby() {
        PhotonNetwork.LeaveLobby();
    }

    public void JoinMatch(string matchName)
    {
        PhotonNetwork.JoinRoom(matchName);
    }

    /// <summary>
    /// Changes the selected map.
    /// </summary>
    /// <param name="name">Scene name</param>
    public void SelectMap(string name)
    {
        //Debug.Log("Selected map " + name);
        _selectedMap = name;
        if (PhotonNetwork.inRoom)
        {
            Hashtable updatedProperties = new Hashtable() { { "SelectedMap", _selectedMap } };
            PhotonNetwork.room.SetCustomProperties(updatedProperties);
        }
    }

    //Sets the player's status ready and updates the UI
    public void PlayerReady()
    {
        //Debug.Log(PhotonNetwork.player.NickName + " ready.");
        int shipID = FindObjectOfType<ShipManager>().GetSelectedShip();
        Hashtable playerProperties = new Hashtable() { { "Ready", "true" }, {"SelectedShip", shipID }, {"Kills", 0 } };
        PhotonNetwork.player.SetCustomProperties(playerProperties); //Callback OnPlayerPropertiesChanged

        //TODO: Disable player and match name changing

        //TODO: Disable ship selection
    }

    //Sets the player's status not ready and updates the UI
    public void PlayerNotReady()
    {
        Hashtable playerProperties = new Hashtable() { { "Ready", "false" } };
        PhotonNetwork.player.SetCustomProperties(playerProperties); //Callback OnPlayerPropertiesChanged

        //TODO: Enable player and match name changing

        //TODO: Enable ship selection
    }

    #endregion

    #region Callbacks

    public override void OnCreatedRoom()
    {
        //Debug.Log("Player name " + PhotonNetwork.playerName);
        FindObjectOfType<MenuManager>().SetMatchName(PhotonNetwork.room.CustomProperties["MatchName"].ToString());
    }

    public override void OnJoinedRoom()
    {
        string players = "";
        foreach (var player in PhotonNetwork.playerList) {
            players += player.NickName + " ";
        }
        //Debug.Log(PhotonNetwork.player + " joined room. Players in room: " + players);

        GeneratePlayerNameIfEmpty();
        PlayerNotReady();

        if (PhotonNetwork.player.IsLocal && !PhotonNetwork.isMasterClient) // = Player is in the Join Game menu
        {
            //Update matchlist player count
            FindObjectOfType<PhotonMatchList>().UpdatePlayerCount(PhotonNetwork.room);
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
       //Debug.Log(PhotonNetwork.player.NickName + " left room.");
        if (SceneManager.GetActiveScene().name != "1_Main_Menu")
        {
            PhotonNetwork.LoadLevel("1_Main_Menu");
        }
    }

    /// <summary>
    /// Generate an unique name for player if the player leaves the name field empty.
    /// </summary>
    public void GeneratePlayerNameIfEmpty()
    {
        if (string.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            PhotonNetwork.playerName = "Player " + PhotonNetwork.room.PlayerCount;
            OnPlayerNameChanged();
        }
    }

    /// <summary>
    /// Update the UI when player changes his/her name.
    /// </summary>
    public void OnPlayerNameChanged()
    {
        //Update UI
        PhotonPlayerName nameField = FindObjectOfType<PhotonPlayerName>();
        if (nameField)
        {
            nameField.UpdatePlayerName(PhotonNetwork.playerName);
        }
    }

    ///<summary>
    /// Called when player's custom properties are changed. Used for handling player transitions between states Ready/Not ready.
    /// </summary>
    public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        if (PhotonNetwork.inRoom && SceneManager.GetActiveScene().name == "1_Main_Menu")
        {
            Hashtable props = playerAndUpdatedProps[1] as Hashtable;

            string players = "";
            foreach (var player in PhotonNetwork.playerList)
            {
                players += player.NickName + " ";
            }
            //Debug.Log(PhotonNetwork.player + " properties changed. Players in room: " + players);

            //Update UI
            PhotonPlayerlist.Instance.UpdatePlayerlist();

            //Check if all the players are ready
            if (PhotonNetwork.isMasterClient)
            {
                // If match is full...
                if (PhotonNetwork.playerList.Length == PhotonNetwork.room.MaxPlayers)
                {
                    //... check if all the players are now ready
                    if (props != null && props.ContainsKey("Ready"))
                    {
                        bool allReady = true;
                        foreach (var player in PhotonNetwork.playerList)
                        {
                            if (!Convert.ToBoolean(player.CustomProperties["Ready"]))
                            {
                                allReady = false;
                            }
                        }

                        //If all the players are ready, load the game level.
                        if (allReady)
                        {
                            PhotonNetwork.room.IsOpen = false;

                            //Masterclient sets the spawn order for the players
                            int spawnpoint = 0;
                            foreach (var player in PhotonNetwork.playerList)
                            {
                                Hashtable playerProperties = new Hashtable() { { "Spawnpoint", spawnpoint } };
                                player.SetCustomProperties(playerProperties);
                                spawnpoint++;
                            }
                            LoadLevel(PhotonNetwork.room.CustomProperties["SelectedMap"].ToString());
                        }
                    }
                }
            }
        }
    }


    public void LoadLevel(string name) {
        if (PhotonNetwork.isMasterClient)
        {
            //TODO: hard coded build indexes... Any better ideas?
            int buildIndex = 0;
            int.TryParse(name.Substring(0, 1), out buildIndex);
            PhotonNetwork.LoadLevel(buildIndex);
        }
    }
    #endregion
}
