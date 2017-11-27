using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TorpedoPowerUp : PowerUp {

    public float RecoilForce;

    void Awake() {
        controllerReference = null;
        mode = PowerUpMode.Spawn;
    }

    [PunRPC]
    public void assignSource(int viewId, int objectId)
    {
        PhotonView.Find(objectId).gameObject.GetComponent<UseTorpedo>().source = PhotonView.Find(viewId).gameObject;
        Debug.Log(PhotonView.Find(objectId).gameObject.GetComponent<UseTorpedo>().source);

    }
    public override void UseNormalPowerUp()
    {
        GameObject go = PhotonNetwork.Instantiate("Torpedo", Ship.LocalPlayerInstance.transform.position, Ship.LocalPlayerInstance.transform.rotation, 0);
        go.transform.rotation = Ship.LocalPlayerInstance.transform.rotation;
        go.GetComponent<Rigidbody2D>().velocity = Ship.LocalPlayerInstance.GetComponent<Rigidbody2D>().velocity * Time.deltaTime * 5;
        photonView.RPC("assignSource", PhotonTargets.All, Ship.LocalPlayerInstance.GetPhotonView().viewID, go.GetPhotonView().viewID);
        Units--;
        Recoil();
    }

    //[ClientRpc]
    //TODO: Refactor with [PunRPC]
    //  public override void RpcSpawnPowerUp(GameObject controller, NetworkInstanceId id) {
    //      GameObject player = ClientScene.FindLocalObject(id);
    //      controller.GetComponent<Rigidbody2D>().velocity = player.GetComponent<Rigidbody2D>().velocity;
    //controller.GetComponent<UseTorpedo>().source = player;
    //      if (player != GameManager.Instance.Player) {
    //          controller.layer = 12;
    //      } else {
    //          Recoil();
    //      }
    //  }

    void Recoil() {
        Ship.LocalPlayerInstance.GetComponent<Rigidbody2D>().AddForce(Ship.LocalPlayerInstance.transform.up * RecoilForce, ForceMode2D.Impulse);
    }
}
