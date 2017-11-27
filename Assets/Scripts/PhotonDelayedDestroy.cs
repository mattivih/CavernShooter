using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonDelayedDestroy : MonoBehaviour {


    private void Awake()
    {
        Invoke("DestroyDelayed", 2.0f);
       // GetComponent<PhotonView>().RPC("DestroyDelayed", PhotonTargets.All, null);
    }

    [PunRPC]
    public void DestroyDelayed()
    {
        if(GetComponent<PhotonView>().isMine)
            PhotonNetwork.Destroy(gameObject);
    }


	
	// Update is called once per frame
	void Update () {
		
	}
}
