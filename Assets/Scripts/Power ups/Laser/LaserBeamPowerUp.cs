using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LaserBeamPowerUp : PowerUp
{

    public GameObject LaserBeamPrefab;

    private GameObject _laser;

    void Awake()
    {
        spawn = LaserBeamPrefab;
        controllerReference = typeof(UseLaserBeam);
        mode = PowerUpMode.Controller;
        Debug.Log("3");
    }

    /*[PunRPC]
    public void RpcStop() {
        if (_laser && _laser.GetComponent<UseLaserBeam>()) {
            _laser.GetComponent<UseLaserBeam>().Stop();
        }
    }*/

    public override void UseContinuousPowerUp()
    {
        
        player = GameManager.Instance.Player;
        _laser = Instantiate(LaserBeamPrefab, player.transform.position, player.transform.rotation);
        UseLaserBeam laser = _laser.GetComponent<UseLaserBeam>();
        laser.LaserPowerUp = this;
        laser.SetParent(player.GetComponent<Ship>().PowerUpPosition.transform);
        laser.Firepoint = player.GetComponent<Ship>().PowerUpPosition.transform;
        laser.Fire();
        if (player != GameManager.Instance.Player)
        {
            laser.LayerMask = laser.LayerMaskEnemy;
        }
        else
        {
            laser.LayerMask = laser.LayerMaskPlayer;
        }
    }

    public override void Stop()
    {
        if (_laser && _laser.GetComponent<UseLaserBeam>())
        {
            _laser.GetComponent<UseLaserBeam>().Stop();
        }
    }
}
//[PUNRPC]
//public override void RpcSpawnPowerUp(GameObject controller) {
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
