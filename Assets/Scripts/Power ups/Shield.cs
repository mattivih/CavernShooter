using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Shield : PowerUp
{

    public GameObject ShieldEffect;
    public float ShieldAmount = 0.25f;

    void Awake()
    {
        stacking = StackMode.Shield;
    }

    public override void UseNormalPowerUp()
    {
        if(GameManager.Instance.Player.GetComponent<Ship>().Shield + GameManager.Instance.Player.GetComponent<Ship>().MaxHealth * ShieldAmount > GameManager.Instance.Player.GetComponent<Ship>().MaxHealth)
            GameManager.Instance.Player.GetComponent<Ship>().Shield = GameManager.Instance.Player.GetComponent<Ship>().MaxHealth;
        else
            GameManager.Instance.Player.GetComponent<Ship>().Shield += GameManager.Instance.Player.GetComponent<Ship>().MaxHealth * ShieldAmount;
    }

    public override void RpcUseNormalPowerUp(NetworkInstanceId id)
    {
        GameObject i = ClientScene.FindLocalObject(id);
        for(int j = 0; j < i.transform.childCount; j++)
        {
            if (i.transform.GetChild(j).name == "ShieldEffect(Clone)")
                return;
        }
     
        GameObject o = GameObject.Instantiate(ShieldEffect, i.transform);
        o.transform.localPosition = new Vector3(0, i.GetComponent<Ship>().PowerUpEffectYOffSet, -1);
    }
}

