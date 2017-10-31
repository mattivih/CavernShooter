using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthPack : PowerUp {

	public GameObject HealthEffect;
    public float HealthRegenPercentage = 0.3f;
	public float effectDuration = 3f;

  /*  void Awake() {
        stacking = StackMode.Shield;
        audioActivate = AddAudio(clipActivate, false, false, 1f);
    }

    public override void UseNormalPowerUp() {
        GameObject p = Ship.LocalPlayerInstance;
        if (p.GetComponent<Ship>().Health + p.GetComponent<Ship>().MaxHealth * HealthRegenPercentage <= p.GetComponent<Ship>().MaxHealth)
            p.GetComponent<Ship>().IncreaseHealth(p.GetComponent<Ship>().MaxHealth * HealthRegenPercentage);
        
        //CmdIncreaseHealth(GameManager.Instance.Player.GetComponent<NetworkIdentity>().netId);
    }

 //   [Command]
 //   void CmdIncreaseHealth(NetworkInstanceId id) {
 //       GameObject p = NetworkServer.FindLocalObject(id);
 //       p.GetComponent<Ship>().RpcIncreaseHealth(p.GetComponent<Ship>().MaxHealth * HealthRegenPercentage);
 //   }

	public override void RpcUseNormalPowerUp(NetworkInstanceId id){
        GameObject i = Ship.LocalPlayerInstance;
        AudioSource.PlayClipAtPoint(clipActivate, i.transform.position);
        GameObject o = PhotonNetwork.Instantiate("HealthEffect", i.transform.position, Quaternion.identity, 0);
        o.transform.localPosition = new Vector3(0, i.GetComponent<Ship>().PowerUpEffectYOffSet, -1);
        Destroy(o, effectDuration);
    }
    */
}

