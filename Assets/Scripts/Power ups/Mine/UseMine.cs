using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseMine : Photon.PunBehaviour {


	private GameObject _base;
	private GameObject _rotating;
	public float MineLifetime = 3f;
	public GameObject source = null;
	public ParticleSystem MineExplosionPrefab;
    private GameObject m;

    public void photonDestroyExplosion()
    {
        PhotonNetwork.Destroy(m);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void disableMine(int objectID, int viewId)
    {
        PhotonView.Find(viewId).gameObject.GetComponent<Ship>().TakeDamage(10f, source);
        PhotonView.Find(objectID).gameObject.SetActive(false);
    }

    /// <summary>
    /// Blows up mine if enemy ship comes too close to the mine, and gives enemy ship damage. Also checks if there is a base within the mines circle of damage.
    /// </summary>
    /// <param name="collider">Collider.</param>
    void OnTriggerEnter2D(Collider2D collider) {   
        if (collider.gameObject.GetComponent<Ship>() && collider.gameObject != source) {
            if (_base)
                _base.GetComponent<Base>().TakeDamage(10f);
                    

            photonView.RPC("disableMine", PhotonTargets.All, photonView.viewID, collider.GetComponent<PhotonView>().viewID);
            Invoke("photonDestroyExplosion", 1f);

            m = PhotonNetwork.Instantiate("MineExplosion", transform.position, Quaternion.identity, 0);
			m.transform.position += m.transform.forward * -4f;                     
          
    
		} else if (collider.tag == "Base") {
			_base = collider.gameObject;
		}
	}
}
