using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ZeroGravityGenerator : PowerUp {

	public GameObject ZeroGravityEffect;
    public float ZeroGravityTime = 15f;

    private float _originalGravity;
    float originalGravity;

    void Awake()
    {
        audioActivate = AddAudio(clipActivate, false, false, 1f);
    }

    [PunRPC]
    public void spawnZeroGravity(int viewId, int childId)
    {
        GameObject i = PhotonView.Find(viewId).gameObject;
        GameObject o = PhotonView.Find(childId).gameObject;

        o.transform.SetParent(i.transform);
        o.transform.localPosition = new Vector3(0, i.GetComponent<Ship>().PowerUpEffectYOffSet, -1);


    }


    public override void UseNormalPowerUp()
    {
        ZeroGravityHelper++;
        GameManager.Instance.hud.ResetZeroGravity();

        if (Ship.LocalPlayerInstance.transform.GetComponent<Rigidbody2D>().gravityScale != 0)       
            originalGravity = Ship.LocalPlayerInstance.transform.GetComponent<Rigidbody2D>().gravityScale;

        Ship.LocalPlayerInstance.transform.GetComponent<Rigidbody2D>().gravityScale = 0;
        zGravityOn = true;
        Invoke("NormalGravity", ZeroGravityTime);


        GameObject i = Ship.LocalPlayerInstance;
        AudioSource.PlayClipAtPoint(clipActivate, i.transform.position);
        for (int j = 0; j < i.transform.childCount; j++)
        {
            if (i.transform.GetChild(j).name == "ZeroGravityEffect(Clone)")
                return;
        }

        var go = PhotonNetwork.Instantiate("ZeroGravityEffect", Vector3.zero, Quaternion.identity, 0);
        photonView.RPC("spawnZeroGravity", PhotonTargets.All, Ship.LocalPlayerInstance.GetPhotonView().viewID, go.GetPhotonView().viewID);

        Units--;
    }

    public void NormalGravity()
    {
        if(ZeroGravityHelper < 2)
        {
            Ship.LocalPlayerInstance.transform.GetComponent<Rigidbody2D>().gravityScale = originalGravity;
            foreach(Transform child in Ship.LocalPlayerInstance.transform)
            {
                if (child.name == "ZeroGravityEffect(Clone)")
                    PhotonNetwork.Destroy(child.gameObject);
            }
            

        }
            
        
        ZeroGravityHelper--;
    }

      /* public override void RpcUseNormalPowerUp(NetworkInstanceId id){
        if (GameManager.Instance.powerupBarImage.fillAmount > 0.01f)
        {
            GameObject i = Ship.LocalPlayerInstance;
            AudioSource.PlayClipAtPoint(clipActivate, i.transform.position);
            for (int j = 0; j < i.transform.childCount; j++)
            {
                if (i.transform.GetChild(j).name == "ZeroGravityEffect(Clone)")
                    return;
            }
            GameObject o = PhotonNetwork.Instantiate("ZeroGravityEffect", i.transform.position, Quaternion.identity, 0);
            o.transform.localPosition = new Vector3(0, i.GetComponent<Ship>().PowerUpEffectYOffSet, -1);
            Destroy(o, ZeroGravityTime);
        }

    }*/
}
