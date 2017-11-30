using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipImageSelector : Photon.PunBehaviour {



    public Sprite[] discoveryShuttle;
    public Sprite[] retroWing;
    public Sprite[] teslaRossa;
    public Sprite[] ufo;
    public Sprite[] untiedFighter;

    private int color;
    private Sprite[] currentTable;
    private PhotonPlayer thisPlayer;
    private PhotonPlayer[] players;

    public void GetInfo()
    {
        players = PhotonNetwork.playerList;
       
        foreach (PhotonPlayer p in players)
        {
            if (p.NickName == GetComponentInParent<Text>().text)
            {
                thisPlayer = p;
                color = p.ID - 1;
            }
        }


        if (thisPlayer.CustomProperties["SelectedShip"].ToString() == "2")
        {
            currentTable = discoveryShuttle;
        }
        else if (thisPlayer.CustomProperties["SelectedShip"].ToString() == "1")
        {
            currentTable = retroWing;
        }
        else if (thisPlayer.CustomProperties["SelectedShip"].ToString() == "3")
        {
            currentTable = ufo;
        }
        else if (thisPlayer.CustomProperties["SelectedShip"].ToString() == "4")
        {
            currentTable = teslaRossa;
        }
        else if (thisPlayer.CustomProperties["SelectedShip"].ToString() == "0")
        {
            currentTable = untiedFighter;
        }

        GetComponent<Image>().sprite = currentTable[color];
    }
}

