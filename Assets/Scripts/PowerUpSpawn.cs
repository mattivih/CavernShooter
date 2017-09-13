using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawn : MonoBehaviour
{

    public List<GameObject> PowerUpPrefabs;

    public Transform CenterSpawn;

    public float CenterTimer = 10.0f;
    public float InnerTimer = 20.0f;
    public float OuterTimer = 30.0f;

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
        //Debug.Log ("powerups count:" + PowerUpPrefabs.Count);
        //Debug.Log ("inner count:" + InnerSpawnPoints.Count);
        //Debug.Log ("outer count:" + OuterSpawnPoints.Count);

        //if (GameManager.Instance.players.Count > 0) {
        //        if (!active) {
        //            NewCenterSpawn();
        //        }
        //        active = true;
        //        if (active) {
        //            CenterTimer -= Time.deltaTime;
        //            InnerTimer -= Time.deltaTime;
        //            OuterTimer -= Time.deltaTime;

        //            if (CenterTimer <= 0) {
        //                if (CenterSpawn.childCount == 0) {
        //                    NewCenterSpawn();
        //                }
        //                CenterTimer = CenterTime;
        //            }

        //            if (InnerTimer <= 0) {
        //                if (InnerSpawnPoints.Count > 0 && InnerCount < 4) {
        //                    NewSpawn(InnerSpawnPoints, PowerUp.SpawnLocation.Inner);
        //                    InnerCount++;
        //                }
        //                InnerTimer = InnerTime;
        //            }

        //            if (OuterTimer <= 0) {
        //                if (OuterSpawnPoints.Count > 0 && OuterCount < 4) {
        //                    NewSpawn(OuterSpawnPoints, PowerUp.SpawnLocation.Outer);
        //                    OuterCount++;
        //                }
        //                OuterTimer = OuterTime;
        //            }
        //        }
        //    }
        //}

        //public GameObject NewPickUp() {
        //    int random = Random.Range(0, PowerUpPrefabs.Count - 1);
        //    //Debug.Log(random);
        //    return PowerUpPrefabs[random];
        //}

        //public void NewCenterSpawn() {
        //    GameObject o = GameObject.Instantiate(NewPickUp(), CenterSpawn);
        //    CenterTimer = CenterTime;
        //    //NetworkServer.Spawn(o);
        //}

        //public void NewSpawn(List<Transform> spawns, PowerUp.SpawnLocation location) {

        //    int randomSpawnIndex = 0;
        //    do {
        //        randomSpawnIndex = Random.Range(0, spawns.Count);
        //    } while (spawns[randomSpawnIndex].childCount > 0);

        //    GameObject o = Instantiate(NewPickUp(), spawns[randomSpawnIndex].transform);
        //    o.GetComponent<PowerUp>().location = location;
        //    //NetworkServer.Spawn(o);
        //}


        //public void ClaimPowerUp(PowerUp.SpawnLocation location) {
        //    switch (location) {
        //        case PowerUp.SpawnLocation.Center:
        //            break;
        //        case PowerUp.SpawnLocation.Inner:
        //            InnerCount--;
        //            break;
        //        case PowerUp.SpawnLocation.Outer:
        //            OuterCount--;
        //            break;
        //        default:
        //            break;
        //    }
        //}


        //[ClientRpc]
        //void RpcMessage(string msg) {
        //    Debug.Log(msg);
        //}
    }
}
