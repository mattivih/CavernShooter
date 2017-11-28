using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonDelayedDestroy : MonoBehaviour {


    private void Awake()
    {
        if (gameObject.GetComponentInChildren<Light>())
            Invoke("DestroyDelayed", 1.0f);
        else
            Invoke("DestroyDelayed", 2.0f);

    }

    [PunRPC]
    public void DestroyDelayed()
    {
        if(GetComponent<PhotonView>().isMine)
            PhotonNetwork.Destroy(gameObject.GetPhotonView());
    }


	
	// Update is called once per frame
	void Update () {
		
	}
}
