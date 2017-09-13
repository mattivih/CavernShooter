using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightningPowerUp : PowerUp {

    public GameObject LightningUsePowerUpPrefab;
    private GameObject controller;
    
    void Awake() {
        spawn = LightningUsePowerUpPrefab;
        controllerReference = typeof(LightningController);
        mode = PowerUpMode.Controller;
    }

    //[ClientRpc]
    //public override void RpcSpawnPowerUp(GameObject controller, NetworkInstanceId id) {
    //    player = ClientScene.FindLocalObject(id);
    //    //Debug.Log("player: " + player + ", id: " + id.Value + ", controller:" + controller);
    //    controller.transform.SetParent(player.transform);
    //    controller.GetComponent<LightningController>().SetParent(player.transform);
    //    controller.GetComponent<LightningController>().StartLightning();
    //    this.controller = controller;

    //    if(player != GameManager.Instance.Player) {
    //        controller.layer = 12;
    //        controller.GetComponent<LightningController>().LayerMask = controller.GetComponent<LightningController>().LayerMaskEnemy;
    //    } else {
    //        controller.GetComponent<LightningController>().LayerMask = controller.GetComponent<LightningController>().LayerMaskPlayer;
    //    }
    //}
    //[ClientRpc]
    //public override void RpcUsePowerUp() {
    //    controller.GetComponent<LightningController>().StartLightning();
    //}
}
