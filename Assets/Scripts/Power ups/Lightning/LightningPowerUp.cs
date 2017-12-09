using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightningPowerUp : PowerUp {

    public GameObject LightningUsePowerUpPrefab;
    private GameObject controller;
    
    void Awake() {
        spawn = LightningUsePowerUpPrefab;
        controllerReference = typeof(LightningController);
        mode = PowerUpMode.Normal;
    }

    public override void UseNormalPowerUp()
    {
        GameObject prefab = PhotonNetwork.Instantiate("Lightning", Ship.LocalPlayerInstance.transform.position, Quaternion.identity, 0);
        prefab.transform.SetParent(Ship.LocalPlayerInstance.transform);
        
        photonView.RPC("StartLightning", PhotonTargets.All, Ship.LocalPlayerInstance.GetPhotonView().viewID, prefab.GetPhotonView().viewID);
        Units--;
    }

    [PunRPC]
    public void StartLightning(int viewID, int objectID)
    {
        GameObject prefab = PhotonView.Find(objectID).gameObject;
        GameObject source = PhotonView.Find(viewID).gameObject;
        LightningController lightning = prefab.GetComponent<LightningController>();
        if (Ship.LocalPlayerInstance.GetPhotonView().viewID == viewID)
        {
            lightning.SetParent(Ship.LocalPlayerInstance.transform);
            lightning.transform.rotation = source.transform.rotation;
            lightning.LayerMask = lightning.LayerMaskPlayer;
            lightning.StartLightning();
        }
        else
        {
            prefab.layer = 12;
            lightning.LayerMask = lightning.LayerMaskEnemy;
            lightning.SetParent(source.transform);
            lightning.transform.rotation = source.transform.rotation;
            lightning.StartLightning();
        }
    }


    //[ClientRpc]
    //public override void RpcSpawnPowerUp(GameObject controller, NetworkInstanceId id) {
    //    player = ClientScene.FindLocalObject(id);
    //    //Debug.Log("player: " + player + ", id: " + id.Value + ", controller:" + controller);
    //    controller.transform.SetParent(player.transform);
    //    controller.GetComponent<LightningController>().SetParent(player.transform);
    //    controller.GetComponent<LightningController>().StartLightning();
    //    this.controller = controller;

    //    
    //}
    //[ClientRpc]
    //public override void RpcUsePowerUp() {
    //    controller.GetComponent<LightningController>().StartLightning();
    //}
}
