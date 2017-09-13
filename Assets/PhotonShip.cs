using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonShip : Photon.PunBehaviour, IPunObservable {

    void Update()
    {
        if (!photonView.isMine && PhotonNetwork.connected)
        {
            //TODO: Tag enemies
            return;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new NotImplementedException();
    }
}
