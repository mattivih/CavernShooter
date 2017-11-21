using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonServerConnection : Photon.PunBehaviour {

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        FindObjectOfType<MenuManager>().OnConnectedToServer();
    }
}
