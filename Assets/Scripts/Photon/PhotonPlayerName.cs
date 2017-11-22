
using System;
using UnityEngine;
using UnityEngine.UI;

public class PhotonPlayerName : MonoBehaviour
{

    public void SetPlayerName()
    {
        PhotonNetwork.playerName = GetComponent<InputField>().text;
    }

    public void UpdatePlayerName(string name)
    {
        GetComponent<InputField>().text = name;
    }
}

