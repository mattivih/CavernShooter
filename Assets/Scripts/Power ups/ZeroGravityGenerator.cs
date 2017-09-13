using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



public class ZeroGravityGenerator : PowerUp {

	public GameObject ZeroGravityEffect;
    public float ZeroGravityTime = 15f;
    float originalGravity;

    void Awake() {
        dieDelay = true;
        audioActivate = AddAudio(clipActivate, false, false, 1f);
      
    }

    public override void UseNormalPowerUp() {
        isUsed = true;
        GameManager.Instance.powerupBarImage.fillAmount = 1;    
        if (GameManager.Instance.Player.GetComponent<Rigidbody2D>().gravityScale != 0) {
            originalGravity = GameManager.Instance.Player.GetComponent<Rigidbody2D>().gravityScale;   
        }

        GameManager.Instance.Player.GetComponent<Rigidbody2D>().gravityScale = 0;
        zGravityOn = true;
        if (zGravityOn)
            zGravityOn = false;
        Invoke("NormalGravity", ZeroGravityTime);    
        readyToDie = false;
    }

    public void NormalGravity() {
        if (zGravityOn)
        {
            GameManager.Instance.Player.GetComponent<Rigidbody2D>().gravityScale = originalGravity;
            readyToDie = true;
            Die();
            zGravityOn = false;
        }
    
    }

	public override void RpcUseNormalPowerUp(NetworkInstanceId id){
        if (GameManager.Instance.powerupBarImage.fillAmount > 0.01f)
        {
               GameObject i = ClientScene.FindLocalObject (id);
        AudioSource.PlayClipAtPoint(clipActivate, i.transform.position);
		GameObject o = GameObject.Instantiate (ZeroGravityEffect, i.transform);
		o.transform.localPosition = new Vector3 (0, i.GetComponent<Ship> ().PowerUpEffectYOffSet, -1);
		Destroy (o, ZeroGravityTime);
        }
     
	}

}
