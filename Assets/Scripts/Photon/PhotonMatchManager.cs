using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonMatchManager : Photon.PunBehaviour {
    
    public static PhotonMatchManager Instance;

    private string[] _shipNames = { "Untied Fighter", "Retro-Wing", "Discovery Shuttle", "U.F.O", "Tesla Rossa" };

    

    public void Start()
    {
        if (Instance == null) {
            Instance = this;
        }

        if (SceneManager.GetActiveScene().name != "0_Title_Screen" || SceneManager.GetActiveScene().name != "1_Main_Menu")
        {
            int shipID = 0;
            int.TryParse(PhotonNetwork.player.CustomProperties["SelectedShip"].ToString(), out shipID);
            string shipName = _shipNames[shipID];

            int spawnpoint = 0;
            int.TryParse(PhotonNetwork.player.CustomProperties["Spawnpoint"].ToString(), out spawnpoint);

            GameObject[] spawnpoints = GameObject.FindGameObjectsWithTag("Base");

            Vector3 spawnPos = spawnpoints[spawnpoint].GetComponent<Transform>().position + Vector3.up;

            //Instantiate player
            if (Ship.LocalPlayerInstance == null)
            {
                GameObject player = PhotonNetwork.Instantiate(shipName, spawnPos, Quaternion.identity, 0);
                MeshFilter meshFilter = player.GetComponentInChildren<MeshFilter>();
                float shipHeight = (meshFilter.mesh.bounds.size.y * meshFilter.gameObject.transform.localScale.y) / 2;
                player.transform.position = player.transform.position + Vector3.up * shipHeight;
                player.name = PhotonNetwork.player.NickName;
            }
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
