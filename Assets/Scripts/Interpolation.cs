using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Interpolation : NetworkBehaviour {
    [SyncVar]
    Vector3 realPosition = Vector3.zero;
    [SyncVar]
    Quaternion realRotation;

    void Update() {
        if (isLocalPlayer) {
            realPosition = transform.position;
            realRotation = transform.rotation;
            CmdSync(transform.position, transform.rotation);
        } else {
            //transform.position = Vector3.Lerp(transform.position, realPosition, Time.deltaTime);
            //transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, Time.deltaTime);
            transform.position = realPosition;
            transform.rotation = realRotation;
        }
    }

    [Command]
    void CmdSync(Vector3 position, Quaternion rotation) {
        RpcSync(transform.position, transform.rotation);
    }

    [ClientRpc]
    void RpcSync(Vector3 position, Quaternion rotation) {
        if (isLocalPlayer) return;
        realPosition = position;
        realRotation = rotation;
    }
}
