﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : ProjectilesBase {

    //public float Damage = 50f; Inherited from ProjectileBaseu
    public CircleCollider2D DestructionCircle;

    public bool clientSide = false;
    //float distanceThreshold = 0.01f;
    //float toofarThreshold = 1f;

    public GameObject Source;

    public Color myColor = Color.white;

    public AudioClip clipHitTerrain, clipHitShip;
    private AudioSource audioHitTerrain, audioHitShip;

    #region To be deleted: Old Unet variables
    //public GameObject serverObj = null;
    //public GameObject clientObj = null;
    //bool serverObjSet = false;
    //[SerializeField]
    //float syncTimer = 0;
    #endregion


    void Awake() {
        audioHitTerrain = AddAudio(clipHitTerrain, false, false, 1f);
        audioHitShip = AddAudio(clipHitShip, false, false, 1f);
    }   


    void Start() {
        DestructionCircle.enabled = false;
        //GetComponent<SpriteRenderer>().material.SetColor("_Color", myColor);
        float h, s, v;
        Color.RGBToHSV(myColor, out h, out s, out v);
        s *= 0.9f;
        //GetComponent<SpriteRenderer>().material.SetColor("_MKGlowColor", Color.HSVToRGB(h, s, v));
    }

    /// <summary>
    /// 1. turns projectile in the direction of movement smoothly
    /// 2. destroyes object if it's below 1000 (no stray projectiles)
    /// </summary>
    void Update() {
        Vector2 moveDirection = GetComponent<Rigidbody2D>().velocity;
        if (moveDirection != Vector2.zero) {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
            Quaternion newRot = Quaternion.AngleAxis(angle, Vector3.forward);
            if (transform.rotation != newRot) {
                transform.rotation = newRot;
            }
        }
        if (transform.position.y < -1000) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Handles laser collisions.
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter2D(Collision2D collision) {
        GameObject collider = collision.gameObject;
        if (collider.tag == "Level") {
            //DestructionCircle.enabled = true;
            //collision.gameObject.GetComponentInParent<GroundController>().DestroyGround(DestructionCircle);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Renderer>().enabled = false;
            //GetComponent<SpriteRenderer>().enabled = false;
            //Debug.Log("Laser hit terrain");
            audioHitTerrain.Play();
            Destroy(gameObject, audioHitTerrain.clip.length);
        } else if (collider.GetComponent<Ship>()) {
            //Debug.Log("Laser of " + Source.GetComponent<Ship>().GetComponent<PhotonView>().viewID + " hit player " + collider.GetComponent<Ship>().GetComponent<PhotonView>().viewID + " Laser layer: " + LayerMask.LayerToName(gameObject.layer) + " Target layer: " + LayerMask.LayerToName(collider.gameObject.layer));
            audioHitShip.Play();
            //Debug.Log("Damage player");
            float dmgMultiplier = Source.GetComponent<Ship>().DamageMultiplier;
            collider.GetComponent<Ship>().TakeDamage(Damage * dmgMultiplier, Source);
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Renderer>().enabled = false;
            Destroy(gameObject, audioHitShip.clip.length);
        } else {
            Destroy(gameObject);
        }
    }

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip;
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }
}
