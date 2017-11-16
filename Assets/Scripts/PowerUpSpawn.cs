using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpSpawn : Photon.PunBehaviour
{

    private bool _firstspawn = true;
    public List<GameObject> PowerUpPrefabs;
    public Transform CenterSpawn;
    public float CenterTimer = 10.0f;
    public float InnerTimer = 20.0f;
    public float OuterTimer = 30.0f;

    public float[] InnerTimers = new float[4] { 5f, 5f, 5f, 5f };
    public float[] OuterTimers = new float[4] { 5f, 5f, 5f, 5f };

    public float[] FirstInnerTimers = new float[4] { 5f, 10f, 15f, 20f };
    public float[] FirstOuterTimers = new float[4] { 5f, 10f, 15f, 20f };

    public float CenterTime = 10.0f;
    public float InnerTime = 20.0f;
    public float OuterTime = 30.0f;

    public List<Transform> InnerSpawnPoints;
    public List<Transform> OuterSpawnPoints;

    public int InnerCount = 0;
    public int OuterCount = 0;

    bool active = false;

    // Update is called once per frame
    void Update()
    {

        //Debug.Log("inner count:" + InnerCount);
        //Debug.Log("outer count:" + OuterCount);

        if (PhotonNetwork.isMasterClient && PhotonNetwork.playerList.Length > 0)
        {
            if (!active)
            {
                NewCenterSpawn(NewPickUp());
            }
            active = true;
            if (active)
            {
                CenterTimer -= Time.deltaTime;
                InnerTimer -= Time.deltaTime;
                OuterTimer -= Time.deltaTime;

                if (CenterTimer <= 0)
                {
                    if (CenterSpawn.childCount == 0)
                    {
                        NewCenterSpawn(NewPickUp());
                    }
                    CenterTimer = CenterTime;
                }

                if (InnerTimer <= 0)
                {
                    if (InnerCount < 4)
                    {
                        NewSpawn(NewPickUp(), InnerSpawnPoints, PowerUp.SpawnLocation.Inner);
                        InnerCount++;
                    }
                    InnerTimer = InnerTime;
                }


                if (OuterTimer <= 0)
                {
                    if (OuterSpawnPoints.Count > 0 && OuterCount < 4)
                    {
                        NewSpawn(NewPickUp(), OuterSpawnPoints, PowerUp.SpawnLocation.Outer);
                        OuterCount++;
                    }
                    OuterTimer = OuterTime;
                }
            }
        }
    }
    
    public int NewPickUp()
    {
        int random = Random.Range(0, PowerUpPrefabs.Count);
        return random;
    }

    public void NewCenterSpawn(int powerup)
    {
        GameObject pickup = PhotonNetwork.Instantiate(PowerUpPrefabs[7].name, CenterSpawn.position, Quaternion.identity, 0);
        pickup.transform.SetParent(CenterSpawn);
        CenterTimer = CenterTime;
    }

    public void NewSpawn(int powerup, List<Transform> spawns, PowerUp.SpawnLocation location)
    {
        int randomSpawnIndex = 0;
        do
        {
            randomSpawnIndex = Random.Range(0, spawns.Count);
        } while (spawns[randomSpawnIndex].childCount > 0);

        GameObject pickup = PhotonNetwork.Instantiate(PowerUpPrefabs[powerup].name, spawns[randomSpawnIndex].position, Quaternion.identity, 0);
        pickup.transform.SetParent(spawns[randomSpawnIndex]);
        pickup.gameObject.GetComponent<PowerUp>().location = location;
    }

    public void ClaimPowerUp(int location)
    {
        switch (location)
        {
            case 1:
                InnerCount--;
                break;
            case 2:
                OuterCount--;
                break;
            default:
                break;
        }
    }
}
