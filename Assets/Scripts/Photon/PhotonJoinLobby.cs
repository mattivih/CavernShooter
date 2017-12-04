using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonJoinLobby : MonoBehaviour {

    public void OnEnable()
    {
        PhotonLobbyManager.Instance.JoinLobby();
    }

    public void OnDisable()
    {
        PhotonLobbyManager.Instance.ExitLobby();
    }
}
