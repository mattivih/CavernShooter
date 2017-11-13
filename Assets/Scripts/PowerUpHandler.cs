using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpHandler : Photon.PunBehaviour
{
    public GameObject CurrentPowerUp;
    private GameObject _powerUpIconHUDPos;
    private GameObject _currentPowerUpIcon;
    private PowerUpSpawn _spawner;
    public List<GameObject> _powerUpList;
    public AudioClip clipDepleted, clipPickUp;
    private AudioSource audioDepleted, audioPickUp;
    //public override void OnStartClient() {
    //    _powerUpIconHUDPos = GameObject.Find("PowerUpIcon");
    //}

    void Start()
    //public override void OnStartClient()
    {

        _powerUpIconHUDPos = GameObject.Find("PowerUpIcon");
        _spawner = FindObjectOfType<PowerUpSpawn>();

        GameObject laser = GameObject.Find("LaserPowerUp");
        _powerUpList.Add(laser);
        GameObject distortionRay = GameObject.Find("DistortionRayPowerUp");
        _powerUpList.Add(distortionRay);
        GameObject health = GameObject.Find("HealthPowerUp");
        _powerUpList.Add(health);
        GameObject mine = GameObject.Find("MinePowerUp");
        _powerUpList.Add(mine);
        GameObject shield = GameObject.Find("ShieldPowerUp");
        _powerUpList.Add(shield);
        GameObject torpedo = GameObject.Find("TorpedoPowerUp");
        _powerUpList.Add(torpedo);
        GameObject flamethrower = GameObject.Find("FlamethrowerPowerUp");
        _powerUpList.Add(flamethrower);
        GameObject zeroGravity = GameObject.Find("ZeroGravityPowerUp");
        _powerUpList.Add(zeroGravity);
    }

    void Awake()
    {
        audioDepleted = AddAudio(clipDepleted, false, false, 1f);
        audioPickUp = AddAudio(clipPickUp, false, false, 1f);
    }

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol)
    {
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
   
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (photonView.isMine)
        {
            if (collider.tag == "PowerUp")
            {
               
                foreach (GameObject powerup in _powerUpList)
                {
                    if (collider.GetComponent<PowerUp>().GetType() == powerup.GetComponent<PowerUp>().GetType())
                    {
                        audioPickUp.Play();
                        CurrentPowerUp = powerup;
                        powerup.GetComponent<PowerUp>().Units = powerup.GetComponent<PowerUp>().MaxUnits;
                        ClaimPrefab(collider.gameObject);
                        photonView.RPC("DestroyPickUp", PhotonTargets.MasterClient, collider.gameObject.GetComponent<PhotonView>().viewID);
                    }
                }
                if (_currentPowerUpIcon)
                {
                    Destroy(_currentPowerUpIcon);
                }
                _currentPowerUpIcon = Instantiate(CurrentPowerUp.GetComponent<PowerUp>().Icon, _powerUpIconHUDPos.transform, false);


                if (CurrentPowerUp && CurrentPowerUp.GetType() == collider.GetComponent<PowerUp>().GetType())
                {
                    PowerUp powerup = CurrentPowerUp.GetComponent<PowerUp>();
                    switch (powerup.stacking)
                    {
                        case PowerUp.StackMode.Add:
                            ClaimPrefab(collider.gameObject);
                            photonView.RPC("DestroyPickUp", PhotonTargets.MasterClient, collider.gameObject.GetComponent<PhotonView>().viewID);
                            break;
                        case PowerUp.StackMode.None:
                            break;
                        case PowerUp.StackMode.Shield:
                            if (powerup.GetType() == typeof(Shield))
                            {
                                //powerup.Use(GetComponent<NetworkIdentity>().netId);
                                ClaimPrefab(collider.gameObject);
                                photonView.RPC("DestroyPickUp", PhotonTargets.MasterClient, collider.gameObject.GetComponent<PhotonView>().viewID);
                            }
                            else
                            {
                                Debug.LogError("Stacking mode is shield without being shield prefab");
                            }
                            break;
                        default:
                            break;
                    }
                }
                GameManager.Instance.UpdatePowerUp();
            }

        }
         
    }
    public void ClaimPrefab(GameObject prefab)
    {
        if ((int)prefab.GetComponent<PowerUp>().location == 1)
        {
            photonView.RPC("RPCClaimPrefab", PhotonTargets.MasterClient, 1);
        }
        else if ((int)prefab.GetComponent<PowerUp>().location == 2)
        {
            photonView.RPC("RPCClaimPrefab", PhotonTargets.MasterClient, 2);
        }
    }
    public void Use()
    {
        if (CurrentPowerUp && CurrentPowerUp.GetComponent<PowerUp>().Units > 0)
        {
            CurrentPowerUp.GetComponent<PowerUp>().Use();
        }
        else {
            audioDepleted.Play();
        }
    }

    public void Stop()
    {
        if (CurrentPowerUp)
        {
            CurrentPowerUp.GetComponent<PowerUp>().Stop();
        }
    }

    [PunRPC]
    public void DestroyPickUp(int viewID)
    {
        PhotonNetwork.Destroy(PhotonView.Find(viewID));
    }

    [PunRPC]
    public void RPCClaimPrefab(int location)
    {
        _spawner.ClaimPowerUp(location);
    }

    void Update()
    {
        if (CurrentPowerUp && CurrentPowerUp.GetComponent<PowerUp>().Units <= 0)
        {
            CurrentPowerUp = null;
            _currentPowerUpIcon.GetComponent<Image>().enabled = false;
        }
    }


    //[Command]
    //public void CmdKillPowerUp(NetworkInstanceId powerUpId) {
    //    NetworkServer.Destroy(NetworkServer.FindLocalObject(powerUpId));
    //}

    //public void SetId(NetworkInstanceId id, NetworkInstanceId powerUpId) {
    //    GameObject powerUp = ClientScene.FindLocalObject(powerUpId);
    //    powerUp.GetComponent<PowerUp>().ownerId = id;
    //    CmdSetId(id, powerUpId);
    //}

    //[Command]
    //void CmdSetId(NetworkInstanceId id, NetworkInstanceId powerUpId) {
    //    RpcSetId(id, powerUpId);
    //}
    //[ClientRpc]
    //void RpcSetId(NetworkInstanceId id, NetworkInstanceId powerUpId) {
    //    Debug.Log("Setting player id " + id);
    //    GameObject powerUp = ClientScene.FindLocalObject(powerUpId);
    //    powerUp.GetComponent<PowerUp>().ownerId = id;
    //    transform.SetParent(null);
    //}

    //[Command]
    //void CmdHidePowerUp(NetworkInstanceId id) {
    //    RpcHidePowerUp(id);
    //}
    //[ClientRpc]
    //void RpcHidePowerUp(NetworkInstanceId id) {
    //    GameObject o = ClientScene.FindLocalObject(id);
    //    o.GetComponentInChildren<Renderer>().enabled = false;
    //    o.GetComponentInChildren<Collider2D>().enabled = false;
    //    o.transform.parent = null;
    //}
    //[Command]
    //void CmdSetAuthority(NetworkIdentity grabID) {
    //    grabID.AssignClientAuthority(connectionToClient);
    //}

    //[Command]
    //void CmdClaimPrefab(PowerUp.SpawnLocation loc) {
    //    GameObject.FindGameObjectWithTag("PowerUpSpawner").GetComponent<PowerUpSpawn>().ClaimPowerUp(loc);
    //}



    //    public void CmdKillPowerUp(NetworkInstanceId powerUpId)
    //    {
    //        NetworkServer.Destroy(NetworkServer.FindLocalObject(powerUpId));
    //    }

    //    public void SetId(NetworkInstanceId id, NetworkInstanceId powerUpId)
    //    {
    //        GameObject powerUp = ClientScene.FindLocalObject(powerUpId);
    //        powerUp.GetComponent<PowerUp>().ownerId = id;
    //        CmdSetId(id, powerUpId);
    //    }

    //    [Command]
    //    void CmdSetId(NetworkInstanceId id, NetworkInstanceId powerUpId)
    //    {
    //        RpcSetId(id, powerUpId);
    //    }
    //    [ClientRpc]
    //    void RpcSetId(NetworkInstanceId id, NetworkInstanceId powerUpId)
    //    {
    //        Debug.Log("Setting player id " + id);
    //        GameObject powerUp = ClientScene.FindLocalObject(powerUpId);
    //        powerUp.GetComponent<PowerUp>().ownerId = id;
    //        transform.SetParent(null);
    //    }

    //    [Command]
    //    void CmdHidePowerUp(NetworkInstanceId id)
    //    {
    //        RpcHidePowerUp(id);
    //    }
    //    [ClientRpc]
    //    void RpcHidePowerUp(NetworkInstanceId id)
    //    {
    //        GameObject o = ClientScene.FindLocalObject(id);
    //        o.GetComponentInChildren<Renderer>().enabled = false;
    //        o.GetComponentInChildren<Collider2D>().enabled = false;
    //        o.transform.parent = null;
    //    }
    //    [Command]
    //    void CmdSetAuthority(NetworkIdentity grabID)
    //    {
    //        grabID.AssignClientAuthority(connectionToClient);
    //    }

    //    [Command]
    //    void CmdClaimPrefab(PowerUp.SpawnLocation loc)
    //    {
    //        GameObject.FindGameObjectWithTag("PowerUpSpawner").GetComponent<PowerUpSpawn>().ClaimPowerUp(loc);
    //    }

    //    public void Use()
    //    {
    //        if (CurrentPowerUp)
    //        {
    //            CurrentPowerUp.GetComponent<PowerUp>().Use(GetComponent<NetworkIdentity>().netId);
    //        }
    //        else
    //        {
    //            audioDepleted.Play();
    //        }
    //    }

    //    public void Stop()
    //    {
    //        if (CurrentPowerUp)
    //        {
    //            CurrentPowerUp.GetComponent<PowerUp>().Stop();
    //        }
    //    }
    //
}
