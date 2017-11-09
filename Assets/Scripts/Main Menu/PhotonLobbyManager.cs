using System.Collections;
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

    public void CreateMatch(string name, int playerCount)
    {
        JoinLobby();
        Debug.Log("@CreateMatch Name: \"" + name + "\" Players: " +  playerCount);
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.CreateRoom(name, new RoomOptions() {MaxPlayers = (byte) playerCount}, null);
        }
    }

    /// <summary>
    /// Joins the lobby - start receiving matchlist updates.
    /// </summary>
    public void JoinLobby() {
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable() { { "ReadyToBegin", "false" } };
        PhotonNetwork.player.SetCustomProperties(playerProperties);
        Debug.Log("Ready To Begin: " + PhotonNetwork.player.CustomProperties["ReadyToBegin"]);
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

    #region Callbacks
    public override void OnConnectedToMaster()
    {
            base.OnConnectedToMaster();
            Debug.Log("@OnConnectedToMaster()");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("@OnJoinedRoom(). This client is in a room now.");

        //TODO: For testing. Refactor to load the level after all the players are marked ready
        if (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers)
        {
            PhotonNetwork.LoadLevel(_selectedMap);
        }
        else
        {
            //Update matchlist player count
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        base.OnDisconnectedFromPhoton();
        Debug.LogWarning("OnDisconnectedFromPhoton() was called by PUN");
    }

    #endregion
}
