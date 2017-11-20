using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonMatchManager : Photon.PunBehaviour {
    
    public static PhotonMatchManager Instance;
    public GameObject CameraPrefab;

    public void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (Instance == null) {
            Instance = this;
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "0_Title_Screen" || scene.name != "1_Main_Menu")
        {
            //Instantiate player
            if (Ship.LocalPlayerInstance == null)
            {
                //TODO: replace vector3 with one of the spawn points
                GameObject player = PhotonNetwork.Instantiate(PhotonNetwork.player.CustomProperties["SelectedShip"].ToString(), Vector3.zero, Quaternion.identity, 0);
                player.name = PhotonNetwork.player.NickName;
            }
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
