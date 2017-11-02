using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthPack : PowerUp {

	public GameObject HealthEffect;
    public float HealthRegenPercentage = 0.3f;
	public float effectDuration = 3f;
    private GameObject go;

    void Awake()
    {
        stacking = StackMode.Shield;
        audioActivate = AddAudio(clipActivate, false, false, 1f);
    }


    [PunRPC]
    public void spawnHealthEffect(int viewId, int childId)
    {
        GameObject i = PhotonView.Find(viewId).gameObject;
        GameObject o = PhotonView.Find(childId).gameObject;

        o.transform.SetParent(i.transform);
        o.transform.localPosition = new Vector3(0, i.GetComponent<Ship>().PowerUpEffectYOffSet, -1);
    }

    public override void UseNormalPowerUp() {
        GameObject p = Ship.LocalPlayerInstance;
        if (p.GetComponent<Ship>().Health + p.GetComponent<Ship>().MaxHealth * HealthRegenPercentage <= p.GetComponent<Ship>().MaxHealth)
            p.GetComponent<Ship>().IncreaseHealth(p.GetComponent<Ship>().MaxHealth * HealthRegenPercentage);
        else
            p.GetComponent<Ship>().Health = p.GetComponent<Ship>().MaxHealth;
        Units--;

        AudioSource.PlayClipAtPoint(clipActivate, p.transform.position);
        go = PhotonNetwork.Instantiate("HealthEffect", Vector3.zero, Quaternion.identity, 0);

        photonView.RPC("spawnHealthEffect", PhotonTargets.All, p.GetPhotonView().viewID, go.GetPhotonView().viewID);
        Invoke("destroyWithDelay", effectDuration);

    }

    public void destroyWithDelay()
    {
        PhotonNetwork.Destroy(go);
    }

 //   [Command]
 //   void CmdIncreaseHealth(NetworkInstanceId id) {
 //       GameObject p = NetworkServer.FindLocalObject(id);
 //       p.GetComponent<Ship>().RpcIncreaseHealth(p.GetComponent<Ship>().MaxHealth * HealthRegenPercentage);
 //   }

	/*public override void RpcUseNormalPowerUp(NetworkInstanceId id){
        GameObject i = Ship.LocalPlayerInstance;
        AudioSource.PlayClipAtPoint(clipActivate, i.transform.position);
        GameObject o = PhotonNetwork.Instantiate("HealthEffect", i.transform.position, Quaternion.identity, 0);
        o.transform.localPosition = new Vector3(0, i.GetComponent<Ship>().PowerUpEffectYOffSet, -1);
        Destroy(o, effectDuration);
    }
    */
}

