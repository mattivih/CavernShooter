using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ZeroGravityGenerator : PowerUp {

	public GameObject ZeroGravityEffect;
    public float ZeroGravityTime = 20f;

    private float _originalGravity;
    private bool _dieDelay, _isUsed, _readyToDie;
    float originalGravity;

    void Awake() {
        _dieDelay = true;
       // audioActivate = AddAudio(clipActivate, false, false, 1f);
    }

    //public override void UseNormalPowerUp()
    //{
    //    if (GameManager.Instance.Player.GetComponent<Rigidbody2D>().gravityScale != 0)
    //    {
    //        originalGravity = GameManager.Instance.Player.GetComponent<Rigidbody2D>().gravityScale;
    //    }
    //    GameManager.Instance.Player.GetComponent<Rigidbody2D>().gravityScale = 0;
    //    Invoke("NormalGravity", ZeroGravityTime);
    //    readyToDie = false;
    //}

    public void NormalGravity()
    {
        GameManager.Instance.Player.GetComponent<Rigidbody2D>().gravityScale = _originalGravity;
        _readyToDie = true;
        //Die();
    }

    //public override void RpcUseNormalPowerUp(NetworkInstanceId id){
    //       GameObject i = ClientScene.FindLocalObject (id);
    //       AudioSource.PlayClipAtPoint(clipActivate, i.transform.position);
    //	GameObject o = GameObject.Instantiate (ZeroGravityEffect, i.transform);
    //	o.transform.localPosition = new Vector3 (0, i.GetComponent<Ship> ().PowerUpEffectYOffSet, -1);
    //	Destroy (o, ZeroGravityTime);
    //}
    //   public override void UseNormalPowerUp() {
    //       ZeroGravityHelper++;
    //       isUsed = true;
    //       GameManager.Instance.powerupBarImage.fillAmount = 1;    
    //       if (GameManager.Instance.Player.GetComponent<Rigidbody2D>().gravityScale != 0) {
    //           originalGravity = GameManager.Instance.Player.GetComponent<Rigidbody2D>().gravityScale;   
    //       }
    //       GameManager.Instance.Player.GetComponent<Rigidbody2D>().gravityScale = 0;
    //       zGravityOn = true;
    //       Invoke("NormalGravity", ZeroGravityTime);    
    //       readyToDie = false;
    //       delayedDeath();

    //   }

    //   public override void RpcUseNormalPowerUp(NetworkInstanceId id){
    //       if (GameManager.Instance.powerupBarImage.fillAmount > 0.01f)
    //       {
    //           GameObject i = ClientScene.FindLocalObject(id);
    //           AudioSource.PlayClipAtPoint(clipActivate, i.transform.position);
    //           for (int j = 0; j < i.transform.childCount; j++)
    //           {
    //               if (i.transform.GetChild(j).name == "ZeroGravityEffect(Clone)")
    //                   return;
    //           }
    //           GameObject o = GameObject.Instantiate(ZeroGravityEffect, i.transform);
    //           o.transform.localPosition = new Vector3(0, i.GetComponent<Ship>().PowerUpEffectYOffSet, -1);
    //           Destroy(o, ZeroGravityTime);
    //       }

    //}
}
