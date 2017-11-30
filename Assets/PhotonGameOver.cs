using UnityEngine;
using UnityEngine.UI;

public class PhotonGameOver : MonoBehaviour {

    public GameObject ResultList;

    public void OnEnable()
    {
        Text[] textFields = ResultList.GetComponentsInChildren<Text>();

        foreach (var player in PhotonNetwork.playerList)
        {
            int position = 0;
            int.TryParse(player.CustomProperties["Position"].ToString(), out position);

            int kills = 0;
            int.TryParse(player.CustomProperties["Kills"].ToString(), out kills);

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
