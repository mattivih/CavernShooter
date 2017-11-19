
using System;
using UnityEngine;
using UnityEngine.UI;

public class PhotonPlayerName : MonoBehaviour
{

    public void SetPlayerName()
    {
        string name = GetComponent<Text>().text;
       //Debug.Log("Player name set to " + name);
        PhotonNetwork.playerName = name;
    }

    public void UpdatePlayerName(string name)
    {
        GetComponent<InputField>().text = name;
    }
}

