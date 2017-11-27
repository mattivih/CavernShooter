using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : PowerUp
{
    public Coroutine alreadyOn = null;
    public GameObject ShieldEffect;
    public float shieldDuration;

    private ShieldEffect shieldEffect;

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
    public void CreateShield(int viewID)
    {
        PhotonView.Find(viewID).gameObject.GetComponent<Ship>().Shield = true;
    }

   public override void UseNormalPowerUp()
   {      
        photonView.RPC("CreateShield", PhotonTargets.All, Ship.LocalPlayerInstance.GetComponent<PhotonView>().viewID);
        Units--;
        GameManager.Instance.hud.ResetShield();
        if (shieldEffect == null)
        {
            var go = PhotonNetwork.Instantiate("ShieldEffect", Vector3.zero, Quaternion.identity, 0);
            photonView.RPC("spawnShield", PhotonTargets.All, Ship.LocalPlayerInstance.GetPhotonView().viewID, go.GetPhotonView().viewID);

            shieldEffect = go.GetComponent<ShieldEffect>();
            shieldEffect.parent = this;

            alreadyOn = StartCoroutine(shieldEffect.Die());
        }

        else
        {
            shieldEffect.gameObject.GetComponent<AudioSource>().Play();
            StopCoroutine(alreadyOn);
            alreadyOn = StartCoroutine(shieldEffect.Die());
        }
   }
}


