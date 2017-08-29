using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Shield : PowerUp {

	public GameObject ShieldEffect;
    public float ShieldAmount = 0.25f;

    void Awake() {
        stacking = StackMode.Shield;
    }

    public override void UseNormalPowerUp() {
        GameManager.Instance.Player.GetComponent<Ship>().Shield = GameManager.Instance.Player.GetComponent<Ship>().MaxHealth * ShieldAmount;
    }

	public override void RpcUseNormalPowerUp(NetworkInstanceId id){
		GameObject i = ClientScene.FindLocalObject (id);
		GameObject o = GameObject.Instantiate (ShieldEffect, i.transform);
		o.transform.localPosition = new Vector3 (0, i.GetComponent<Ship> ().PowerUpEffectYOffSet, -1);
	}
}

