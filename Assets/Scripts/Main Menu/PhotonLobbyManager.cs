using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonLobbyManager : Photon.PunBehaviour
{
    #region Public Variables

    public static PhotonLobbyManager Instance;

    [Tooltip("The maximum number of players per room.")]
    public byte MaxPlayersPerRoom = 4;
    public PhotonLogLevel Loglevel = PhotonLogLevel.ErrorsOnly;
    #endregion


    #region Private Variables
    private int _selectedMap = 2;

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
        //Debug.Log("@CreateMatch Name: \"" + name + "\" Players: " +  playerCount);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte) playerCount;

        if (string.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            PhotonNetwork.playerName = "Player 1";
            OnPlayerNameChanged();
        }
        if (name == "")
        {
            name = "Match";
        }

        ExitGames.Client.Photon.Hashtable matchProperties = new ExitGames.Client.Photon.Hashtable() { { "MatchName", name } };
        roomOptions.CustomRoomPropertiesForLobby = new[] {"MatchName"};
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
         _selectedMap = SceneManager.GetSceneByName(name).buildIndex;
    }

#endregion

    #region Callbacks
    public override void OnConnectedToMaster()
    {
            base.OnConnectedToMaster();
            Debug.Log("@OnConnectedToMaster()");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("@OnCreatedRoom Room name: " + PhotonNetwork.room.Name + " Room view name: " +  PhotonNetwork.room.CustomProperties["MatchName"]);
    }

    public override void OnJoinedRoom()
    {
        // Set Player ready status
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable() { { "Ready", "false" } };
        PhotonNetwork.player.SetCustomProperties(playerProperties);

        if (PhotonNetwork.player.IsLocal && !PhotonNetwork.isMasterClient) // = Player is in the Join Game menu
        {
            //Update matchlist player count
            FindObjectOfType<PhotonMatchlist>().UpdatePlayerCount(PhotonNetwork.room);
        }
        GeneratePlayerNameIfEmpty();
    }

    public void GeneratePlayerNameIfEmpty()
    {
        if (PhotonNetwork.playerName == "")
        {
            PhotonNetwork.playerName = "Player " + PhotonNetwork.room.PlayerCount + 1;
            OnPlayerNameChanged();
        }
    }

    public void OnPlayerNameChanged()
    {
        //Update UI
        PhotonPlayerName nameField = FindObjectOfType<PhotonPlayerName>();
        Debug.Log("Name field: " + nameField);
        if (nameField)
        {
            nameField.UpdatePlayerName(PhotonNetwork.playerName);
        }
    }

    public override void OnLeftRoom()
    {
        LoadLevel(0); //Lobby
    }

    ///<summary>
    /// Used for handling player transitions between states Ready/Not ready
    /// </summary>
    public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
        Hashtable props = playerAndUpdatedProps[1] as Hashtable;

        if (props != null && props["Ready"] != null) {
            PhotonPlayerlist.Instance.UpdatePlayerStatus(player, Convert.ToBoolean(props["Ready"]));

            //Check if all the players are now ready
            bool allReady = true;
            foreach (var matchPlayer in PhotonNetwork.playerList) {
                if (!Convert.ToBoolean(matchPlayer.CustomProperties["Ready"])) {
                    allReady = false;
                }
            }

            //If all the players are ready, load the game level.
            if (allReady && PhotonNetwork.isMasterClient) {
                LoadLevel(_selectedMap);
            }
        }
    }

    public void LoadLevel(int buildIndex) {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel(buildIndex);
        }
    }


    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        Debug.Log(other.NickName + " disconnected.");
        LoadLevel(SceneManager.GetSceneByName("1_MainMenu").buildIndex);
    }

    public override void OnDisconnectedFromPhoton()
    {
        base.OnDisconnectedFromPhoton();
        Debug.LogWarning("OnDisconnectedFromPhoton() was called by PUN");
    }

    #endregion
}
