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

        if (PhotonNetwork.isMasterClient && PhotonNetwork.playerList.Length > 0)
        {
            if (!active)
                NewCenterSpawn();

            active = true;
            if (active)
            {
                if (CenterSpawn.childCount == 0)
                    CenterTimer -= Time.deltaTime;


                if (CenterTimer <= 0)
                {
                    if (CenterSpawn.childCount == 0)
                    {
                        NewCenterSpawn();
                    }
                    CenterTimer = CenterTime;
                }
                if (_firstspawn)
                {
                    FirstInnerTimers[0] -= Time.deltaTime;
                    FirstInnerTimers[1] -= Time.deltaTime;
                    FirstInnerTimers[2] -= Time.deltaTime;
                    FirstInnerTimers[3] -= Time.deltaTime;

                    FirstOuterTimers[0] -= Time.deltaTime;
                    FirstOuterTimers[1] -= Time.deltaTime;
                    FirstOuterTimers[2] -= Time.deltaTime;
                    FirstOuterTimers[3] -= Time.deltaTime;

                    if (FirstInnerTimers[0] <= 0)
                    {
                        NewSpawn(InnerSpawnPoints[0], PowerUp.SpawnLocation.Inner);
                        FirstInnerTimers[0] = 100f;
                    }
                    if (FirstInnerTimers[1] <= 0)
                    {
                        NewSpawn(InnerSpawnPoints[1], PowerUp.SpawnLocation.Inner);
                        FirstInnerTimers[1] = 100f;
                    }
                    if (FirstInnerTimers[2] <= 0)
                    {
                        NewSpawn(InnerSpawnPoints[2], PowerUp.SpawnLocation.Inner);
                        FirstInnerTimers[2] = 100f;
                    }
                    if (FirstInnerTimers[3] <= 0)
                    {
                        NewSpawn(InnerSpawnPoints[3], PowerUp.SpawnLocation.Inner);
                        FirstInnerTimers[3] = 100f;
                    }

                    if (FirstOuterTimers[0] <= 0)
                    {
                        NewSpawn(OuterSpawnPoints[0], PowerUp.SpawnLocation.Outer);
                        FirstOuterTimers[0] = 100f;
                    }
                    if (FirstOuterTimers[1] <= 0)
                    {
                        NewSpawn(OuterSpawnPoints[1], PowerUp.SpawnLocation.Outer);
                        FirstOuterTimers[1] = 100f;
                    }
                    if (FirstOuterTimers[2] <= 0)
                    {
                        NewSpawn(OuterSpawnPoints[2], PowerUp.SpawnLocation.Outer);
                        FirstOuterTimers[2] = 100f;
                    }
                    if (FirstOuterTimers[3] <= 0)
                    {
                        NewSpawn(OuterSpawnPoints[3], PowerUp.SpawnLocation.Outer);
                        FirstOuterTimers[3] = 100f;
                        _firstspawn = false;
                    }


                    for (int i = 0; i < 4; i++)
                    {
                        if (InnerSpawnPoints[i].childCount == 0 && FirstInnerTimers[i] > 50f)
                            InnerTimers[i] -= Time.deltaTime;
                        if (OuterSpawnPoints[i].childCount == 0 && FirstOuterTimers[i] > 50f)
                            OuterTimers[i] -= Time.deltaTime;

                        if (InnerTimers[i] <= 0)
                        {
                            if (InnerSpawnPoints.Count > 0 && InnerCount < 4)
                            {
                                NewSpawn(InnerSpawnPoints[i], PowerUp.SpawnLocation.Inner);
                                InnerCount++;
                            }
                            InnerTimers[i] = InnerTime;
                        }
                        if (OuterTimers[i] <= 0)
                        {
                            if (OuterSpawnPoints.Count > 0 && OuterCount < 4)
                            {
                                NewSpawn(OuterSpawnPoints[i], PowerUp.SpawnLocation.Outer);
                                OuterCount++;
                            }
                            OuterTimers[i] = OuterTime;
                        }

                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (InnerSpawnPoints[i].childCount == 0)
                            InnerTimers[i] -= Time.deltaTime;
                        if (OuterSpawnPoints[i].childCount == 0)
                            OuterTimers[i] -= Time.deltaTime;

                        if (InnerTimers[i] <= 0)
                        {
                            if (InnerSpawnPoints.Count > 0 && InnerCount < 4)
                            {
                                NewSpawn(InnerSpawnPoints[i], PowerUp.SpawnLocation.Inner);
                                InnerCount++;
                            }
                            InnerTimers[i] = InnerTime;
                        }
                        if (OuterTimers[i] <= 0)
                        {
                            if (OuterSpawnPoints.Count > 0 && OuterCount < 4)
                            {
                                NewSpawn(OuterSpawnPoints[i], PowerUp.SpawnLocation.Outer);
                                OuterCount++;
                            }
                            OuterTimers[i] = OuterTime;
                        }

                    }
                }
            }
        }
    }
    

    public string NewPickUp()
    {
        int random = Random.Range(0, PowerUpPrefabs.Count);
        return PowerUpPrefabs[random].name;
    }

    public void NewCenterSpawn()
    {
        GameObject pickup = PhotonNetwork.Instantiate(PowerUpPrefabs[5].name, CenterSpawn.position, Quaternion.identity, 0);
        pickup.transform.SetParent(CenterSpawn);
        CenterTimer = CenterTime;
    }

    public void NewSpawn(Transform spawn, PowerUp.SpawnLocation location)
    {
        GameObject pickup = PhotonNetwork.Instantiate(NewPickUp(), spawn.transform.position, Quaternion.identity, 0);
        pickup.transform.SetParent(spawn.transform);
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
