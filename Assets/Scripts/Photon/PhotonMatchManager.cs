using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonMatchManager : Photon.PunBehaviour {
    
    public static PhotonMatchManager Instance;

    private string[] _ships = { "Untied Fighter", "Retro-Wing", "Discovery Shuttle", "U.F.O", "Tesla Rossa" };

    public void Start()
    {
        if (Instance == null) {
            Instance = this;
        }
        if (SceneManager.GetActiveScene().name != "0_Title_Screen" || SceneManager.GetActiveScene().name != "1_Main_Menu")
        {
            int shipID = 0;
            int.TryParse(PhotonNetwork.player.CustomProperties["SelectedShip"].ToString(), out shipID);
            Debug.Log("Instantiating " + PhotonNetwork.player + " with ship " + _ships[shipID]);

            //Instantiate player
            if (Ship.LocalPlayerInstance == null)
            {
                //TODO: replace vector3 with one of the spawn points
                //Debug.Log("Instantiating " + PhotonNetwork.player +" with ship " + _ships[shipID]);
                GameObject player = PhotonNetwork.Instantiate(_ships[shipID], Vector3.zero, Quaternion.identity, 0);
                player.name = PhotonNetwork.player.NickName;
            }
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
