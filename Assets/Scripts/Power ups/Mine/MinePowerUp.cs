using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinePowerUp : PowerUp {

	public GameObject MinePrefab;
    
    public Sprite[] playerTextures;
    void Awake() {
        spawn = MinePrefab;
        controllerReference = null;
        mode = PowerUpMode.Spawn;
    }

    [PunRPC]
    public void assignSource(int viewId, int objectId)
    {
        PhotonView.Find(objectId).gameObject.GetComponent<UseMine>().source = PhotonView.Find(viewId).gameObject;

        if (viewId == 1001)
        {
            PhotonView.Find(objectId).gameObject.GetComponent<SpriteRenderer>().sprite = playerTextures[0];
        }
        else if (viewId == 2001)
        {
            PhotonView.Find(objectId).gameObject.GetComponent<SpriteRenderer>().sprite = playerTextures[1];
        }
        else if (viewId == 3001)
        {
            PhotonView.Find(objectId).gameObject.GetComponent<SpriteRenderer>().sprite = playerTextures[2];
        }
        else if (viewId == 4001)
        {
            PhotonView.Find(objectId).gameObject.GetComponent<SpriteRenderer>().sprite = playerTextures[3];
        }
    }

    public override void UseNormalPowerUp()
    {             
        var spawnPos = (Ship.LocalPlayerInstance.transform.position - (Ship.LocalPlayerInstance.transform.up.normalized * 0.5f));
        int layerMask = 1 << 10;
        Collider2D[] cols = Physics2D.OverlapCircleAll(new Vector2(spawnPos.x, spawnPos.y), 1.2f * 0.15f, layerMask);
        if (cols.Length > 0)
            spawnPos = Ship.LocalPlayerInstance.transform.position;

        GameObject go = PhotonNetwork.Instantiate("Mine", spawnPos, Quaternion.identity, 0);
        go.GetComponent<AudioSource>().Play();
        photonView.RPC("assignSource", PhotonTargets.All, Ship.LocalPlayerInstance.GetPhotonView().viewID, go.GetPhotonView().viewID);  
        Units--;
    }
 
}
