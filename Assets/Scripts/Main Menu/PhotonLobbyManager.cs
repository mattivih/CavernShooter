using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PhotonLobbyManager : Photon.PunBehaviour
{
    #region Public Variables

    public static PhotonLobbyManager Instance;

    [Tooltip("The maximum number of players per room.")]
    public byte MaxPlayersPerRoom = 4;
    public PhotonLogLevel Loglevel = PhotonLogLevel.ErrorsOnly;
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
            // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
            PhotonNetwork.autoJoinLobby = false;


            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.automaticallySyncScene = true;

            PhotonNetwork.logLevel = Loglevel;
    }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
        //Connect();
        if (!Instance)
        {
            Instance = this;
        }
        PhotonNetwork.ConnectUsingSettings(_gameVersion);
    }


    #endregion

        #region Match & Lobby methods

    public void CreateMatch(string name, int playerCount)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte) playerCount;

        if (name == "")
        {
            name = "Match";
        }

        //Initialize custom properties for the player
        PlayerNotReady();

        Hashtable matchProperties = new Hashtable() { { "MatchName", name }, { "SelectedMap", _selectedMap } };
        roomOptions.CustomRoomPropertiesForLobby = new[] {"MatchName"};
        roomOptions.CustomRoomPropertiesForLobby = new[] {"SelectedMap"};
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
        _selectedMap = name;
        if (PhotonNetwork.inRoom)
        {
            Hashtable updatedProperties = new Hashtable() { { "SelectedMap", _selectedMap } };
            PhotonNetwork.room.SetCustomProperties(updatedProperties);
            Debug.Log("Selected map " + PhotonNetwork.room.CustomProperties["SelectedMap"]);
        }
    }

    //Sets the player's status ready and updates the UI
    public void PlayerReady()
    {
        string shipName = FindObjectOfType<ShipManager>().GetSelectedShip();
        Hashtable playerProperties = new Hashtable() { { "Ready", "true" }, {"SelectedShip", shipName} };
        PhotonNetwork.player.SetCustomProperties(playerProperties); //Callback OnPlayerPropertiesChanged
    }

    //Sets the player's status not ready and updates the UI
    public void PlayerNotReady()
    {
        Hashtable playerProperties = new Hashtable() { { "Ready", "false" } };
        PhotonNetwork.player.SetCustomProperties(playerProperties); //Callback OnPlayerPropertiesChanged
    }

    #endregion

    #region Callbacks

    public override void OnCreatedRoom()
    {
        GameObject.Find("Match Name Input").GetComponent<InputField>().text = PhotonNetwork.room.CustomProperties["MatchName"].ToString();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("@OnJoinedRoom");

        GeneratePlayerNameIfEmpty();
        PlayerNotReady();

        if (PhotonNetwork.player.IsLocal && !PhotonNetwork.isMasterClient) // = Player is in the Join Game menu
        {
            //Update matchlist player count
            FindObjectOfType<PhotonMatchlist>().UpdatePlayerCount(PhotonNetwork.room);
        }
    }

    /// <summary>
    /// Generate an unique name for player if the player leaves the name field empty.
    /// </summary>
    public void GeneratePlayerNameIfEmpty()
    {
        if (PhotonNetwork.playerName == "")
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

    public override void OnLeftRoom()
    {
        LoadLevel("1_MainMenu");
    }

    ///<summary>
    /// Called when player's custom properties are changed. Used for handling player transitions between states Ready/Not ready.
    /// </summary>
    public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
        Hashtable props = playerAndUpdatedProps[1] as Hashtable;

        //Update UI
        PhotonPlayerlist.Instance.UpdatePlayerlist();

        Debug.Log(player.NickName + " selected ship " + props["SelectedShip"]);

        //Check if all the players are ready
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient) {
            // If match is full...
            if (PhotonNetwork.playerList.Length == PhotonNetwork.room.MaxPlayers)
            {
                //... check if all the players are now ready
                if (props != null && props.ContainsKey("Ready"))
                {
                    bool allReady = true;
                    foreach (var matchPlayer in PhotonNetwork.playerList)
                    {
                        if (!System.Convert.ToBoolean(matchPlayer.CustomProperties["Ready"]))
                        {
                            allReady = false;
                        }
                    }

                    //If all the players are ready, load the game level.
                    if (allReady)
                    {
                        LoadLevel(PhotonNetwork.room.CustomProperties["SelectedMap"].ToString());
                    }
                }
            }
        }
    }


    public void LoadLevel(string name) {
        Debug.Log("Loading level " + name);
        if (PhotonNetwork.isMasterClient)
        {
            //TODO: hard coded build indexes... Any better ideas?
            int buildIndex = 0;
            int.TryParse(name.Substring(0, 1), out buildIndex);
            PhotonNetwork.LoadLevel(buildIndex);
        }
    }


    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        Debug.Log(other.NickName + " disconnected.");
        LoadLevel("1_MainMenu");
    }

    public override void OnDisconnectedFromPhoton()
    {
        base.OnDisconnectedFromPhoton();
        Debug.LogWarning("OnDisconnectedFromPhoton() was called by PUN");
    }

    #endregion
}
