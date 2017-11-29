using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonPlaceHolderLobbyManager : Photon.PunBehaviour
{
    #region Public Variables
    [Tooltip("The maximum number of players per room.")]
    public byte MaxPlayersPerRoom = 4;
    public PhotonLogLevel Loglevel = PhotonLogLevel.ErrorsOnly;
    #endregion


    #region Private Variables
    bool isConnecting = false;

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
            Connect();
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// Start the connection process. 
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.connected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }
    #endregion

    #region Callbacks
    public override void OnConnectedToMaster()
    {
            base.OnConnectedToMaster();
            Debug.Log("@OnConnectedToMaster()");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        base.OnPhotonRandomJoinFailed(codeAndMsg);
        Debug.Log("@OnPhotonRandomJoinFailed(). No random room available, creating one.");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = (byte)MaxPlayersPerRoom }, null);

    }

    public override void OnJoinedRoom()
    {

        base.OnJoinedRoom();
        //Ship.PlayerID = PhotonNetwork.countOfPlayers;
        Debug.Log("@OnJoinedRoom(). This client is in a room now.");

        //Load room only if we are the first player joining.
        if (PhotonNetwork.room.PlayerCount == 1) {
            PhotonNetwork.LoadLevel(1);
        }

    }

    public override void OnDisconnectedFromPhoton()
    {
        base.OnDisconnectedFromPhoton();
        Debug.LogWarning("OnDisconnectedFromPhoton() was called by PUN");
    }

    #endregion
}
