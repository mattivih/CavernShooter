using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlamethrowerPowerUp : PowerUp {

    public GameObject FlamethrowerPrefab;

    private GameObject _controller;

    void Awake() {
        spawn = FlamethrowerPrefab;
        controllerReference = typeof(FlamethrowerCollision);
        mode = PowerUpMode.Controller;
    }

	[ClientRpc]
	public override void RpcStop() {
        if (_controller && _controller.GetComponentInChildren<FlamethrowerCollision>()) {
            _controller.GetComponentInChildren<FlamethrowerCollision>().Stop();
        }
	}

    [ClientRpc]
    public override void RpcSpawnPowerUp(GameObject controller, NetworkInstanceId id) {
        player = ClientScene.FindLocalObject(id);
        controller.GetComponentInChildren<FlamethrowerCollision>()._flamethrowerPowerUp = this;
        controller.GetComponent<Flamethrower>().SetParent(player.transform);
		controller.GetComponentInChildren<FlamethrowerCollision>().currentPrefabScript = controller.GetComponent<FireBaseScript>();
		controller.GetComponentInChildren<FlamethrowerCollision>().BeginEffect(controller);
		_controller = controller;
	}

    [ClientRpc]
    public override void RpcUsePowerUp() {
        if (_controller && _controller.GetComponentInChildren<FlamethrowerCollision>()) {
            _controller.GetComponentInChildren<FlamethrowerCollision>().BeginEffect(_controller);
        } 
    }

}
