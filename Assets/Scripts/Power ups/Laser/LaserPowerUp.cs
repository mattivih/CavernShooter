using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LaserPowerUp : PowerUp {

    public GameObject LaserPrefab;

    private GameObject _laser;

   void Awake() {
        spawn = LaserPrefab;
		controllerReference = typeof(UseLaser);
        mode = PowerUpMode.Controller;
    }
    //[ClientRpc]
    //public override void RpcStop() {
    //    if (_laser && _laser.GetComponent<UseLaser>()) {
    //        _laser.GetComponent<UseLaser>().Stop();
    //    }
    //}

    //[Command]
    //public override void CmdSpawnPowerUp(NetworkInstanceId id) {
    //    player = NetworkServer.FindLocalObject(id);
    //    _laser = Instantiate(LaserPrefab, player.transform.position, player.transform.rotation);
    //    NetworkServer.Spawn(_laser);
    //    RpcSpawnPowerUp(_laser, id);
    //}
    //[ClientRpc]
    //public override void RpcSpawnPowerUp(GameObject controller, NetworkInstanceId id) {
    //    player = ClientScene.FindLocalObject(id);
    //    controller.GetComponent<UseLaser>().LaserPowerUp = this;
    //    controller.GetComponent<UseLaser>().SetParent(player.GetComponent<Ship>().PowerUpPosition.transform);
    //    controller.GetComponent<UseLaser>().Firepoint = player.GetComponent<Ship>().PowerUpPosition.transform;
    //    controller.GetComponent<UseLaser>().Fire();
    //    _laser = controller;
    //    if (player != GameManager.Instance.Player) {
    //        controller.GetComponent<UseLaser>().LayerMask = controller.GetComponent<UseLaser>().LayerMaskEnemy;
    //    } else {
    //        controller.GetComponent<UseLaser>().LayerMask = controller.GetComponent<UseLaser>().LayerMaskPlayer;
    //    }
    //}
    //[ClientRpc]
    //public override void RpcUsePowerUp() {
    //    _laser.GetComponent<UseLaser>().Fire();
    //}
}