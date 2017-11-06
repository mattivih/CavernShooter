using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : PowerUp
{
 
    public GameObject ShieldEffect;
    public float ShieldAmount = 0.25f;

     void Awake()
    {
        stacking = StackMode.Shield;
    }

    [PunRPC]
    public void spawnShield(int viewId, int childId)
    {
        GameObject i = PhotonView.Find(viewId).gameObject;
        GameObject o = PhotonView.Find(childId).gameObject;

        o.transform.SetParent(i.transform);
        o.transform.localPosition = new Vector3(0, i.GetComponent<Ship>().PowerUpEffectYOffSet, -1);
    }

   public override void UseNormalPowerUp()
    {

        if (GameManager.Instance.Player.GetComponent<Ship>().Shield + GameManager.Instance.Player.GetComponent<Ship>().MaxHealth * ShieldAmount > GameManager.Instance.Player.GetComponent<Ship>().MaxHealth)
            GameManager.Instance.Player.GetComponent<Ship>().Shield = GameManager.Instance.Player.GetComponent<Ship>().MaxHealth;
        else
            GameManager.Instance.Player.GetComponent<Ship>().Shield += GameManager.Instance.Player.GetComponent<Ship>().MaxHealth * ShieldAmount;
        Units--;

        for (int j = 0; j < Ship.LocalPlayerInstance.transform.childCount; j++)
        {
            if (Ship.LocalPlayerInstance.transform.GetChild(j).name == "ShieldEffect(Clone)")            
                return;                          
        }
     
        var go = PhotonNetwork.Instantiate("ShieldEffect", Vector3.zero, Quaternion.identity, 0);
        photonView.RPC("spawnShield", PhotonTargets.All, Ship.LocalPlayerInstance.GetPhotonView().viewID, go.GetPhotonView().viewID);

    }

    }


