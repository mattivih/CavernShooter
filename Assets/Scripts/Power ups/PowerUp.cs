using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUp : NetworkBehaviour
{
    
    public float MaxUnits;
    public float Units;
    public GameObject Icon;
    protected GameObject spawn;
    protected System.Type controllerReference;
    protected GameObject player;
    protected PowerUpMode mode = PowerUpMode.Normal;
    public StackMode stacking = StackMode.Add;
    public SpawnLocation location;
    public NetworkInstanceId ownerId;

    public static bool zGravityOn = false;
    public bool readyToDie = true;
    public bool dying = false;
    public bool dieDelay = false;
    public bool isUsed = false;
    public static List<GameObject> goList = new List<GameObject>();

    public AudioClip clipActivate;
    public AudioSource audioActivate;

    public enum PowerUpMode
    {
        Normal,
        Spawn,
        Controller
    }

    public enum StackMode
    {
        Add,
        None,
        Shield,
        ZeroGravity
    }

    public enum SpawnLocation
    {
        Center,
        Inner,
        Outer
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

    void Update()
    {
        if (dying && readyToDie)
        {
            Die();
        }
        if (Units <= 0)
        {
            Die();
        }
    }


    public void Start()
    {
        //Ensure that the icon is always right size
        Icon.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 75);
        Icon.GetComponent<RectTransform>().localScale = Vector3.one;
    }

    /// <summary>
    /// Uses one unit of powerup or starts continuous usage of the power up.
    /// </summary>
    public void Use(NetworkInstanceId id)
    {
        if (Units > 0)
        {
            audioActivate = AddAudio(clipActivate, false, false, 1f);
            audioActivate.Play();
            UsePowerUp(id);
            Units--;
        }
        else
        {
            Die();
        }
    }

    /// <summary>
    /// Stops using the power up if it's a continuous power up
    /// </summary>
    public virtual void Stop()
    {
        if (ownerId.Value != 0)
        {
            readyToDie = false;
            CmdStop();
        }
    }
    [Command]
    public void CmdStop()
    {
        RpcStop();
    }
    [ClientRpc]
    public virtual void RpcStop() { }

    /// <summary>
    /// called when powerup is used up (=units goes to 0)
    /// </summary>
    public void Die()
    {
        if(!dieDelay)
        {
            if (ownerId.Value != 0)
            {
                ClientScene.FindLocalObject(ownerId).GetComponent<PowerUpHandler>().PowerUpDepleted();
                GameManager.Instance.powerupBarLines.enabled = false;
                GameManager.Instance.powerupBarLines4.enabled = false;
            }
            Debug.Log("Die");
            dying = true;
            CmdDie();

        }
    

    }
    [Command]
    public void CmdDie()
    {
        RpcStop();
        RpcDieAfterStop();
    }
    [Command]
    void CmdDieAfterStop()
    {
        if (ownerId.Value != 0)
        {
            Invoke("customDestroy", 0.25f);
            /*  GameObject p = NetworkServer.FindLocalObject(ownerId);
              if (controllerReference != null && p.GetComponentInChildren(controllerReference)) {               
                  NetworkServer.Destroy(p.GetComponentInChildren(controllerReference).gameObject);
              }*/
        }
        //  NetworkServer.Destroy(gameObject);
    }

    public void customDestroy()
    {
            if (controllerReference != null && NetworkServer.FindLocalObject(ownerId).GetComponentInChildren(controllerReference))
            {
                NetworkServer.Destroy(NetworkServer.FindLocalObject(ownerId).GetComponentInChildren(controllerReference).gameObject);
            }
        NetworkServer.Destroy(gameObject);
   
    }

    [ClientRpc]
    void RpcDieAfterStop()
    {
        CmdDieAfterStop();
    }

    /// <summary>
    /// Uses the power up that was previously picked up.
    /// </summary>
    public virtual void UsePowerUp(NetworkInstanceId id)
    {
        GameObject player = GameManager.Instance.Player;
        Vector3 pos = player.transform.position;
        Quaternion rot = player.transform.rotation;
        //Debug.Log("player: " + player.name + ", id: " + id.Value);
        CmdUseNormalPowerUp(id);

        switch (mode)
        {
            case PowerUpMode.Normal:
                UseNormalPowerUp();
                break;
            case PowerUpMode.Spawn:
                CmdSpawnPowerUp(id, pos, rot);
                break;
            case PowerUpMode.Controller:
                if (controllerReference != null)
                {
                    if (!player.GetComponentInChildren(controllerReference))
                    {
                        CmdSpawnPowerUp(id, pos, rot);
                    }
                    else
                    {
                        CmdUsePowerUp();
                    }
                }
                else
                {
                    Debug.LogError("Controller power up without controller reference");
                }
                break;
            default:
                break;
        }
    }

    [Command]
    public void CmdSpawnPowerUp(NetworkInstanceId id, Vector3 position, Quaternion rotation)
    {
        GameObject p = NetworkServer.FindLocalObject(id);
        GameObject controller = Instantiate(spawn, position, rotation);
        NetworkServer.SpawnWithClientAuthority(controller, p);
        RpcSpawnPowerUp(controller, id);
    }
    [ClientRpc]
    public virtual void RpcSpawnPowerUp(GameObject controller, NetworkInstanceId id) { }

    [Command]
    public virtual void CmdUsePowerUp()
    {
        RpcUsePowerUp();
    }
    [ClientRpc]
    public virtual void RpcUsePowerUp() { }

    public virtual void UseNormalPowerUp() { }
    [Command]
    public virtual void CmdUseNormalPowerUp(NetworkInstanceId id)
    {
        RpcUseNormalPowerUp(id);
    }
    [ClientRpc]
    public virtual void RpcUseNormalPowerUp(NetworkInstanceId id) { }

   
    public IEnumerator delayedDeath()
    {
        yield return new WaitForSeconds(15);
        if (ownerId.Value != 0)
            {
                ClientScene.FindLocalObject(ownerId).GetComponent<PowerUpHandler>().PowerUpDepleted();
                GameManager.Instance.powerupBarLines.enabled = false;
                GameManager.Instance.powerupBarLines4.enabled = false;
            }
        Debug.Log("Die");
    }
}
