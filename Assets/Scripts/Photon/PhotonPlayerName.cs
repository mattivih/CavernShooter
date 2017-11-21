
using System;
using UnityEngine;
using UnityEngine.UI;

public class PhotonPlayerName : MonoBehaviour
{

    public void SetPlayerName()
    {
        string name = GetComponent<Text>().text;
        PhotonNetwork.playerName = name;
    }

    public void UpdatePlayerName(string name)
    {
        InputField nameField = GetComponent<InputField>();
        ColorBlock colors = nameField.colors;
        colors.normalColor = Color.white;
        nameField.colors = colors;
        nameField.gameObject.GetComponent<Text>().fontStyle = FontStyle.Normal;
        nameField.text = name;
    }
}

