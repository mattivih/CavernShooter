using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : PowerUp
{

    public GameObject ShieldEffect;
    public float ShieldAmount = 0.25f;

    /* void Awake()
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
        GameObject i = Ship.LocalPlayerInstance;
        for(int j = 0; j < i.transform.childCount; j++)
        {
            if (i.transform.GetChild(j).name == "ShieldEffect(Clone)")
                return;
        }

        GameObject o = PhotonNetwork.Instantiate("ShieldEffect", i.transform.position, Quaternion.identity, 0);
        o.transform.localPosition = new Vector3(0, i.GetComponent<Ship>().PowerUpEffectYOffSet, -1);
        */
    }


