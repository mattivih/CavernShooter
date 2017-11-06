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




    public override void UseNormalPowerUp()
    {
        var go = PhotonNetwork.Instantiate("Mine", Ship.LocalPlayerInstance.transform.position, Quaternion.identity, 0).GetComponent<UseMine>().source = Ship.LocalPlayerInstance;
       // go.GetComponent<UseMine>().source = Ship.LocalPlayerInstance;
       // photonView.RPC("spawnMine", PhotonTargets.All, Ship.LocalPlayerInstance.GetPhotonView().viewID, go.GetPhotonView().viewID);

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
