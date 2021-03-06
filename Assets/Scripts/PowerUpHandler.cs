﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUpHandler : NetworkBehaviour {
    public GameObject CurrentPowerUp;
    private GameObject _powerUpIconHUDPos;
    private GameObject _currentPowerUpIcon;

    public AudioClip clipDepleted, clipPickUp;
    private AudioSource audioDepleted, audioPickUp;

    public override void OnStartClient() {
        _powerUpIconHUDPos = GameObject.Find("PowerUpIcon");
    }

    void Awake() {
        audioDepleted = AddAudio(clipDepleted, false, false, 1f);
        audioPickUp = AddAudio(clipPickUp, false, false, 1f);
    }

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip;
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    /// <summary>
    /// Sets CurrentPowerUp when ship picks up a powerup item.
    /// </summary>
    /// <param name="collider">Collider.</param>
    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "PowerUp" && GetComponent<Ship>().isLocalPlayer) {
            //if player has no power up curretly or powerup hit is differnet type

            if (!CurrentPowerUp || (CurrentPowerUp && CurrentPowerUp.GetComponent<PowerUp>().GetType() != collider.GetComponent<PowerUp>().GetType())) {
                if (CurrentPowerUp && CurrentPowerUp.GetComponent<PowerUp>().GetType() != collider.GetComponent<PowerUp>().GetType()) {
                    PowerUp powerup = CurrentPowerUp.GetComponent<PowerUp>();
                    powerup.Die();
                }
                audioPickUp.Play();
                CmdHidePowerUp(collider.GetComponent<NetworkIdentity>().netId);
                CurrentPowerUp = collider.gameObject;
                CmdSetAuthority(collider.GetComponent<NetworkIdentity>());
                CmdClaimPrefab(collider.gameObject.GetComponent<PowerUp>().location);
                SetId(GetComponent<NetworkIdentity>().netId, collider.GetComponent<NetworkIdentity>().netId);
                //Change icon to HUD
                if (_currentPowerUpIcon) {
                    Destroy(_currentPowerUpIcon);
                }
                _currentPowerUpIcon = Instantiate(CurrentPowerUp.GetComponent<PowerUp>().Icon, _powerUpIconHUDPos.transform, false);
                

            } else if (CurrentPowerUp) {
                PowerUp powerup = CurrentPowerUp.GetComponent<PowerUp>();
                if (powerup.GetType() == collider.GetComponent<PowerUp>().GetType()) {
                    switch (powerup.stacking) {
                        case PowerUp.StackMode.Add:
                            powerup.Units += powerup.MaxUnits;
                            CmdKillPowerUp(collider.GetComponent<NetworkIdentity>().netId);
                            break;
                        case PowerUp.StackMode.None:
                            break;
                        case PowerUp.StackMode.Shield:
                            if (powerup.GetType() == typeof(Shield)) {
                                powerup.Use(GetComponent<NetworkIdentity>().netId);
                            } else {
                                Debug.LogError("Stacking mode is shield without being shield prefab");
                            }
                            break;
                        default:
                            break;
                    }
                    CurrentPowerUp = collider.gameObject;
                    collider.GetComponent<PowerUp>().Die();
                    return;
                } else {
                    powerup.Die();
                }
            }
        }
    }

    public void PowerUpDepleted() {
        Destroy(_currentPowerUpIcon);
        audioDepleted.Play();
    }

    [Command]
    public void CmdKillPowerUp(NetworkInstanceId powerUpId) {
        NetworkServer.Destroy(NetworkServer.FindLocalObject(powerUpId));
    }

    public void SetId(NetworkInstanceId id, NetworkInstanceId powerUpId) {
        GameObject powerUp = ClientScene.FindLocalObject(powerUpId);
        powerUp.GetComponent<PowerUp>().ownerId = id;
        CmdSetId(id, powerUpId);
    }

    [Command]
    void CmdSetId(NetworkInstanceId id, NetworkInstanceId powerUpId) {
        RpcSetId(id, powerUpId);
    }
    [ClientRpc]
    void RpcSetId(NetworkInstanceId id, NetworkInstanceId powerUpId) {
        Debug.Log("Setting player id " + id);
        GameObject powerUp = ClientScene.FindLocalObject(powerUpId);
        powerUp.GetComponent<PowerUp>().ownerId = id;
        transform.SetParent(null);
    }

    [Command]
    void CmdHidePowerUp(NetworkInstanceId id) {
        RpcHidePowerUp(id);
    }
    [ClientRpc]
    void RpcHidePowerUp(NetworkInstanceId id) {
        GameObject o = ClientScene.FindLocalObject(id);
        o.GetComponentInChildren<Renderer>().enabled = false;
        o.GetComponentInChildren<Collider2D>().enabled = false;
        o.transform.parent = null;
    }
    [Command]
    void CmdSetAuthority(NetworkIdentity grabID) {
        grabID.AssignClientAuthority(connectionToClient);
    }

    [Command]
    void CmdClaimPrefab(PowerUp.SpawnLocation loc) {
        GameObject.FindGameObjectWithTag("PowerUpSpawner").GetComponent<PowerUpSpawn>().ClaimPowerUp(loc);
    }

    public void Use() {
        if (CurrentPowerUp) {
            CurrentPowerUp.GetComponent<PowerUp>().Use(GetComponent<NetworkIdentity>().netId);
        } else {
            audioDepleted.Play();
        }
    }

    public void Stop() {
        if (CurrentPowerUp) {
            CurrentPowerUp.GetComponent<PowerUp>().Stop();
        }
    }
}
