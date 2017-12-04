using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonGameOver : MonoBehaviour {

    public GameObject ResultList;

    public void OnEnable()
    {
        Hashtable playerProperties = new Hashtable() { { "Ready", "false" } };
        PhotonNetwork.player.SetCustomProperties(playerProperties);

        Text[] textFields = ResultList.GetComponentsInChildren<Text>();

        foreach (var player in PhotonNetwork.playerList)
        {
            int position = 0;
            if (player.CustomProperties.ContainsKey("Position")) {
                int.TryParse(player.CustomProperties["Position"].ToString(), out position);
                Debug.Log("Parsed position " + position + " for " + player.NickName);
            }

            int kills = 0;
            if (player.CustomProperties.ContainsKey("Kills")) {
                int.TryParse(player.CustomProperties["Kills"].ToString(), out kills);
            }
            Debug.Log(position + ". " + player.NickName + " : " + kills);
            textFields[position - 1].text = player.NickName + " : " + kills;
        }
    }
    public void OnClickBackToMainMenu()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
    }

    public void OnClickQuit()
    {
        PhotonNetwork.Disconnect();
        Application.Quit();
    }
    [PunRPC]
    public void ReloadScene()
    {
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnClickReady()
    {
        Hashtable playerProperties = new Hashtable() { { "Ready", "true" } };
        PhotonNetwork.player.SetCustomProperties(playerProperties);
        GameObject.Find("ReadyChecks").transform.GetChild(PhotonNetwork.player.ID - 1).gameObject.SetActive(true);

        if (PhotonNetwork.isMasterClient)
        {
            if (PhotonNetwork.playerList.Length == PhotonNetwork.room.MaxPlayers)
            {
                bool allReady = true;
                foreach (var player in PhotonNetwork.playerList)
                {
                    if (!Convert.ToBoolean(player.CustomProperties["Ready"]))
                    {
                        allReady = false;
                    }
                }

                if (allReady)
                {
                    PhotonNetwork.room.IsOpen = false;

                    int spawnpoint = 0;
                    foreach (var player in PhotonNetwork.playerList)
                    {
                        Hashtable playerProperties2 = new Hashtable() { { "Spawnpoint", spawnpoint } };
                        player.SetCustomProperties(playerProperties2);
                        spawnpoint++;
                    }
                    GetComponent<PhotonView>().RPC("ReloadScene", PhotonTargets.All);
                }
            }
        }
    }

}
 




