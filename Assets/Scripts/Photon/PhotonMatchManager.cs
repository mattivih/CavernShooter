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
            float baseHeight = spawnpoints[0].gameObject.GetComponent<BoxCollider2D>().size.y * spawnpoints[0].gameObject.transform.localScale.y/2;

            Vector3 spawnPos = spawnpoints[spawnpoint].GetComponent<Transform>().position + Vector3.up * baseHeight;

            //Instantiate player
            if (Ship.LocalPlayerInstance == null)
            {
                GameObject player = PhotonNetwork.Instantiate(shipName, spawnPos, Quaternion.identity, 0);
                player.name = PhotonNetwork.player.NickName;
            }
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
