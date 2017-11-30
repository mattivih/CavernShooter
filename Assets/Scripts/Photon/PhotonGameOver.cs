using UnityEngine;
using UnityEngine.UI;

public class PhotonGameOver : MonoBehaviour {

    public GameObject ResultList;

    public void OnEnable()
    {
        Text[] textFields = ResultList.GetComponentsInChildren<Text>();

        foreach (var player in PhotonNetwork.playerList)
        {
            int position = 1;
            if (player.CustomProperties.ContainsKey("Position")) {
                int.TryParse(player.CustomProperties["Position"].ToString(), out position);
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
}
