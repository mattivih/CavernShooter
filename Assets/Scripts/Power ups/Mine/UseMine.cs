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

    private void Start()
    {
        var colliders = source.GetComponentsInChildren<Collider2D>();
        var colliders2 = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D c in colliders)        
            foreach (Collider2D c2 in colliders2)          
                Physics2D.IgnoreCollision(c, c2);                  
    }

    public void photonDestroyExplosion()
    {
        PhotonNetwork.Destroy(m);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void disableMine(int objectID, int viewId)
    {
        PhotonView.Find(viewId).gameObject.GetComponent<Ship>().TakeDamage(100f, source);
        PhotonView.Find(objectID).gameObject.SetActive(false);
    }

    /// <summary>
    /// Blows up mine if enemy ship comes too close to the mine, and gives enemy ship damage. Also checks if there is a base within the mines circle of damage.
    /// </summary>
    /// <param name="collider">Collider.</param>
    void OnTriggerEnter2D(Collider2D collider) {   
        if (collider.gameObject.GetComponent<Ship>() && collider.gameObject != source) {
            if (_base)
                _base.GetComponent<Base>().TakeDamage(100f);
                    

            photonView.RPC("disableMine", PhotonTargets.All, photonView.viewID, collider.GetComponent<PhotonView>().viewID);
            Invoke("photonDestroyExplosion", 1f);

            m = PhotonNetwork.Instantiate("MineExplosion", transform.position, Quaternion.identity, 0);
			m.transform.position += m.transform.forward * -4f;                     
          
    
		} else if (collider.tag == "Base") {
			_base = collider.gameObject;
		}
	}
}
