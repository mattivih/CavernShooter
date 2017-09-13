using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonPlayerName : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        string defaultName = "Player " + PhotonNetwork.player.ID;
        InputField inputField = GetComponentInChildren<InputField>();
        inputField.text = defaultName;
        PhotonNetwork.playerName = defaultName;
    }

    public void SetPlayerName(string name)
    {
        PhotonNetwork.playerName = name + " "; // force a trailing space string in case value is an empty string, else playerName would not be updated.
    }


}

