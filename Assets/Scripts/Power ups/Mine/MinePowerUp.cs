using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinePowerUp : PowerUp {

	public GameObject MinePrefab;

    public Sprite[] playerTextures;

    void Awake() {
        spawn = MinePrefab;
        controllerReference = null;
        mode = PowerUpMode.Spawn;
    }


    [PunRPC]
    public void assignSource(int viewId, int objectId)
    {
        PhotonView.Find(objectId).gameObject.GetComponent<UseMine>().source = PhotonView.Find(viewId).gameObject;

        if (viewId == 1001)
            PhotonView.Find(objectId).gameObject.GetComponent<SpriteRenderer>().sprite = playerTextures[0];
        else if (viewId == 2001)
            PhotonView.Find(objectId).gameObject.GetComponent<SpriteRenderer>().sprite = playerTextures[1];
        else if (viewId == 3001)
            PhotonView.Find(objectId).gameObject.GetComponent<SpriteRenderer>().sprite = playerTextures[2];
        else if (viewId == 4001)
            PhotonView.Find(objectId).gameObject.GetComponent<SpriteRenderer>().sprite = playerTextures[3];
    }

    public override void UseNormalPowerUp()
    {
        GameObject go = PhotonNetwork.Instantiate("Mine", Ship.LocalPlayerInstance.transform.position, Quaternion.identity, 0);
   
        photonView.RPC("assignSource", PhotonTargets.All, Ship.LocalPlayerInstance.GetPhotonView().viewID, go.GetPhotonView().viewID);
        Units--;
    }


    //  [ClientRpc]
    //  public override void RpcSpawnPowerUp(GameObject controller, NetworkInstanceId id) {
    //      player = ClientScene.FindLocalObject(id);
    //      controller.GetComponent<UseMine>().source = player;
    //      Debug.Log("player: " + player + ", id: " + id.Value + ", controller:" + controller);
    //      controller.GetComponent<SpriteRenderer>().sprite = playerTextures[player.GetComponent<Ship>().playerNum];
    //controller.transform.position = new Vector3 (controller.transform.position.x, controller.transform.position.y, MinePrefab.transform.position.z);
    //  }
}
