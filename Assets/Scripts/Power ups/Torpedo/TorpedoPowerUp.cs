using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TorpedoPowerUp : PowerUp {

    public GameObject TorpedoPrefab;
    public float RecoilForce;

    void Awake() {
        spawn = TorpedoPrefab;
        controllerReference = null;
        mode = PowerUpMode.Spawn;
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
        GameManager.Instance.Player.GetComponent<Rigidbody2D>().AddForce(GameManager.Instance.Player.transform.up * RecoilForce, ForceMode2D.Impulse);
    }
}
