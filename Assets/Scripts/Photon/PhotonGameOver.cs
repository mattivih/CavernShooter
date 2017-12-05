using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonGameOver : MonoBehaviour {

    public GameObject ResultList;

    public void OnEnable()
    {
      
        for (int i = 0; i < GameManager.Instance.hud.transform.childCount-1; ++i)       
            GameManager.Instance.hud.transform.GetChild(i).gameObject.SetActive(false);
        GameManager.Instance.hud.GetComponent<HUDManager>().enabled = false;
        GameManager.Instance.hud.gameObject.SetActive(true);


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
    [PunRPC]
    public void SignalReady(int id)
    {
        GameObject.Find("ReadyChecks").transform.GetChild(id).gameObject.SetActive(true);
    }
    public void OnClickReady()
    {
        Hashtable playerProperties = new Hashtable() { { "Ready", "true" } };
        PhotonNetwork.player.SetCustomProperties(playerProperties);
        GetComponent<PhotonView>().RPC("SignalReady", PhotonTargets.All, PhotonNetwork.player.ID - 1);

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
                    PhotonNetwork.DestroyAll();
                    GetComponent<PhotonView>().RPC("ReloadScene", PhotonTargets.All);
                }
            }
        }
    }

}
 




