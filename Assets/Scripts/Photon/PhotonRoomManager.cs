using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonRoomManager : Photon.PunBehaviour {
    
    static public PhotonRoomManager Instance;

    #region Public Methods

    public void Start() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void LoadGameLevel()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
        PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.room.PlayerCount);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    #endregion

    #region Callbacks

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0); //Lobby
        }

    public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        base.OnPhotonPlayerConnected(other);
        Debug.Log(other.NickName + " joined game.");

        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("@OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);
            LoadGameLevel();
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        Debug.Log(other.NickName + " disconnected.");


        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient);
            LoadGameLevel();
        }
    }


    #endregion
    }
