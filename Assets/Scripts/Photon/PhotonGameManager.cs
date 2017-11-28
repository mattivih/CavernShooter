﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonGameManager : Photon.PunBehaviour {
    
    static public PhotonGameManager Instance;

    public GameObject PlayerPrefab;
    public GameObject CameraPrefab;

    public void Start() {
		

        if (Instance == null) {
            Instance = this;
        }

        //Instantiate player
        if (Ship.LocalPlayerInstance == null)
        {
            //TODO: replace vector3 with one of the spawn points
            //Debug.LogError("Instantiating player.");
            GameObject player = PhotonNetwork.Instantiate(PlayerPrefab.name, Vector3.zero, Quaternion.identity, 0);
            if (PhotonNetwork.room.PlayerCount == 1)
            {
                PhotonNetwork.Instantiate("Base", new Vector3(16.065f, 12.403f, 0f), Quaternion.identity, 0);
                PhotonNetwork.Instantiate("Base", new Vector3(15.89226f, -17.77634f, 0f), Quaternion.identity, 0);
                PhotonNetwork.Instantiate("Base", new Vector3(-16.34225f, 12.38098f, 0f), Quaternion.identity, 0);
                PhotonNetwork.Instantiate("Base", new Vector3(-16.142f, -17.785f, 0f), Quaternion.identity, 0);
            }
         
        }

    }

    public void LoadGameLevel()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        else {
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }



    #region Callbacks

    public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        base.OnPhotonPlayerConnected(other);
        Debug.Log(other.NickName + " " + other.ID + " joined game.");
    }

    /// <summary>
    /// Called when the local player left the room. We need to load the lobby scene.
    /// </summary>
    public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0); //Lobby
        }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        Debug.Log(other.NickName + " disconnected.");


        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient);
            SceneManager.LoadScene(0); //Lobby
        }
    }

    #endregion
    }
