using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthPack : PowerUp {

	public GameObject HealthEffect;
    public float HealthRegenPercentage = 0.3f;
	public float effectDuration = 3f;

    void Awake() {
        stacking = StackMode.Shield;
        audioActivate = AddAudio(clipActivate, false, false, 1f);
    }

    //public override void UseNormalPowerUp() {
    //    //CmdIncreaseHealth(GameManager.Instance.Player.GetComponent<NetworkIdentity>().netId);
    //}

 //   [Command]
 //   void CmdIncreaseHealth(NetworkInstanceId id) {
 //       GameObject p = NetworkServer.FindLocalObject(id);
 //       p.GetComponent<Ship>().RpcIncreaseHealth(p.GetComponent<Ship>().MaxHealth * HealthRegenPercentage);
 //   }
 //   [ClientRpc]
	//public override void RpcUseNormalPowerUp(NetworkInstanceId id){
	//	GameObject i = ClientScene.FindLocalObject (id);
 //       AudioSource.PlayClipAtPoint(clipActivate, i.transform.position);
 //       GameObject o = GameObject.Instantiate (HealthEffect, i.transform);
	//	o.transform.localPosition = new Vector3 (0, i.GetComponent<Ship> ().PowerUpEffectYOffSet, -1);
	//	Destroy (o, effectDuration);
	//}
}

