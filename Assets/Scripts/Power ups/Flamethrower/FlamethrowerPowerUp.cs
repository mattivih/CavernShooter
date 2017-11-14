using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlamethrowerPowerUp : PowerUp {
    private GameObject _controller;
    private GameObject go;

    void Awake() {
        controllerReference = typeof(FlamethrowerCollision);
        mode = PowerUpMode.Controller;
    }

    public override void UseContinuousPowerUp()
    {
        go = PhotonNetwork.Instantiate("FlameThrowerPrefab", Ship.LocalPlayerInstance.transform.transform.position, Ship.LocalPlayerInstance.transform.rotation, 0);
        go.transform.parent = Ship.LocalPlayerInstance.transform;
        go.GetComponentInChildren<FlamethrowerCollision>()._flamethrowerPowerUp = this;
        go.GetComponent<Flamethrower>().SetParent(Ship.LocalPlayerInstance.transform);
        go.GetComponentInChildren<FlamethrowerCollision>().currentPrefabScript = go.GetComponent<FireBaseScript>();
        go.GetComponentInChildren<FlamethrowerCollision>().BeginEffect(go);    
    }

    public override void Stop()
    {
        if (go)
            go.GetComponentInChildren<FlamethrowerCollision>().Stop();       
    }


    //[ClientRpc]
    //public override void RpcStop() {
    //       if (_controller && _controller.GetComponentInChildren<FlamethrowerCollision>()) {
    //           _controller.GetComponentInChildren<FlamethrowerCollision>().Stop();
    //       }
    //}

    //   [ClientRpc]
    //   public override void RpcSpawnPowerUp(GameObject controller, NetworkInstanceId id) {
    //       player = ClientScene.FindLocalObject(id);
    //       controller.GetComponentInChildren<FlamethrowerCollision>()._flamethrowerPowerUp = this;
    //       controller.GetComponent<Flamethrower>().SetParent(player.transform);
    //	controller.GetComponentInChildren<FlamethrowerCollision>().currentPrefabScript = controller.GetComponent<FireBaseScript>();
    //	controller.GetComponentInChildren<FlamethrowerCollision>().BeginEffect(controller);
    //	_controller = controller;
    //}

    //   [ClientRpc]
    //   public override void RpcUsePowerUp() {
    //       if (_controller && _controller.GetComponentInChildren<FlamethrowerCollision>()) {
    //           _controller.GetComponentInChildren<FlamethrowerCollision>().BeginEffect(_controller);
    //       } 
    //   }

}
