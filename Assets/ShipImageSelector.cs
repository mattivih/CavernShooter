using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipImageSelector : Photon.PunBehaviour {

    public HUDManager hud;

    public Sprite[] discoverySprites;
    public Sprite[] retroSprites;
    public Sprite[] teslaSprites;
    public Sprite[] ufoSprites;
    public Sprite[] untiedSprites;

    public int color;
    public Sprite[] currentTable;
    public PhotonPlayer thisPlayer;
    public PhotonPlayer[] players;

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
            currentTable = discoverySprites;
        }
        else if (thisPlayer.CustomProperties["SelectedShip"].ToString() == "1")
        {
            currentTable = retroSprites;
        }
        else if (thisPlayer.CustomProperties["SelectedShip"].ToString() == "3")
        {
            currentTable = ufoSprites;
        }
        else if (thisPlayer.CustomProperties["SelectedShip"].ToString() == "4")
        {
            currentTable = teslaSprites;
        }
        else if (thisPlayer.CustomProperties["SelectedShip"].ToString() == "0")
        {
            currentTable = untiedSprites;
        }

        GetComponent<Image>().sprite = currentTable[color];
    }
}

