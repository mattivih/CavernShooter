﻿using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonMatchManager : Photon.PunBehaviour
{

    public static PhotonMatchManager Instance;
    public GameObject[] Spawnpoints;
    public GameObject GameOverOverlay;

    private string[] _shipNames = { "Untied Fighter", "Retro-Wing", "Discovery Shuttle", "U.F.O", "Tesla Rossa" };
    private int _deadPlayersCount = 0;


    public void Start()
    {
      
        if (Instance == null)
        {
            Instance = this;
        }
        PhotonNetwork.OnEventCall += this.OnEvent;

        #region Instantiate the player
        if (SceneManager.GetActiveScene().name != "0_Title_Screen" || SceneManager.GetActiveScene().name != "1_Main_Menu")
        {
            //Instantiate player
            if (Ship.LocalPlayerInstance == null && PhotonNetwork.player.IsLocal)
            {
                int shipID;
                int.TryParse(PhotonNetwork.player.CustomProperties["SelectedShip"].ToString(), out shipID);
                string shipName = _shipNames[shipID];

                int spawnpoint;
                int.TryParse(PhotonNetwork.player.CustomProperties["Spawnpoint"].ToString(), out spawnpoint);

                float baseHeight = 0.624321f / 2;
                Vector3 spawnPos = Spawnpoints[spawnpoint].GetComponent<Transform>().position + Vector3.up * baseHeight;
                //Debug.Log("Spawning " + PhotonNetwork.player.NickName + " to spawnpoint " + spawnpoint);

                GameObject player = PhotonNetwork.Instantiate(shipName, spawnPos, Quaternion.identity, 0);
                player.name = PhotonNetwork.player.NickName;
            }

            //Instantiate bases
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.Instantiate("Base", new Vector3(16.065f, 12.403f, 0f), Quaternion.identity, 0);
                PhotonNetwork.Instantiate("Base", new Vector3(15.89226f, -17.77634f, 0f), Quaternion.identity, 0);
                PhotonNetwork.Instantiate("Base", new Vector3(-16.34225f, 12.38098f, 0f), Quaternion.identity, 0);
                PhotonNetwork.Instantiate("Base", new Vector3(-16.142f, -17.785f, 0f), Quaternion.identity, 0);
            }
        }
        #endregion 
    }

    public void OnMasterClientOnPlayerDeath(int playerID, int killerID, bool killedByEnemy)
    {

        PhotonPlayer killer = null;
        PhotonPlayer player = null;

        foreach (var photonPlayer in PhotonNetwork.playerList)
        {
            if (killedByEnemy && photonPlayer.ID == killerID)
            {
                killer = photonPlayer;
            }
            if (photonPlayer.ID == playerID)
            {
                player = photonPlayer;
            }
        }

        //Update the killer's kill count.
        if (killedByEnemy && killer != null)
        {
            Hashtable killerProperties = new Hashtable();
            int kills = 0;
            int.TryParse(killer.CustomProperties["Kills"].ToString(), out kills);
            killerProperties.Add("Kills", (kills + 1));
            killer.SetCustomProperties(killerProperties);
        }

        //Save the dead players position.
        Hashtable playerProperties = new Hashtable { { "Position", PhotonNetwork.room.MaxPlayers - _deadPlayersCount } };
        player.SetCustomProperties(playerProperties);
        //Debug.Log("Saved position " + player.CustomProperties["Position"].ToString() + " for " + player.NickName);

        _deadPlayersCount++;

        //If all but 1 player is dead, end the match.
        if (_deadPlayersCount == PhotonNetwork.room.MaxPlayers - 1)
        {
            //If the player was killed by something else than other ship the last man standing is the winner. Find the winner's game object.
            if (!killedByEnemy)
            {
                killer = FindObjectOfType<Ship>().gameObject.GetComponent<PhotonView>().owner; //This is the winner
            }

            //Set Position 1 for the winner
            Hashtable winnerProperties = new Hashtable { { "Position", 1 } };
            killer.SetCustomProperties(winnerProperties);
            //Debug.Log("Saved position " + killer.CustomProperties["Position"].ToString() + " for " + killer.NickName);
            //Show game over overlay on the clients
            GameOverOverlay.SetActive(true);
            PhotonNetwork.RaiseEvent(0, null, true, null);
        }
    }

    /// <summary>
    /// Called on all the clients.
    /// </summary>
    public void OnEvent(byte eventcode, object content, int senderID)
    {
        if (eventcode == 0) {
            //All but 1 player is dead, end the match.   
            GameObject.Find("GameOver").transform.GetChild(0).gameObject.SetActive(true);
           // GameOverOverlay.SetActive(true);
        }
    }

}
 
