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
    [PunRPC]
    public void IncreaseShield(int viewID)
    {

        if (PhotonView.Find(viewID).gameObject.GetComponent<Ship>().Shield + PhotonView.Find(viewID).gameObject.GetComponent<Ship>().MaxHealth * ShieldAmount > PhotonView.Find(viewID).gameObject.GetComponent<Ship>().MaxHealth)
            PhotonView.Find(viewID).gameObject.GetComponent<Ship>().Shield = PhotonView.Find(viewID).gameObject.GetComponent<Ship>().MaxHealth;
        else
            PhotonView.Find(viewID).gameObject.GetComponent<Ship>().Shield += PhotonView.Find(viewID).gameObject.GetComponent<Ship>().MaxHealth * ShieldAmount;
    }

   public override void UseNormalPowerUp()
    {      
        photonView.RPC("IncreaseShield", PhotonTargets.All, Ship.LocalPlayerInstance.GetComponent<PhotonView>().viewID);
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


