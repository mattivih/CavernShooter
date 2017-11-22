using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LaserBeamPowerUp : PowerUp
{
    private bool firing = false;
    private GameObject go;

    void Awake()
    {   
        controllerReference = typeof(UseLaserBeam);
        mode = PowerUpMode.Controller;
    }

    void FixedUpdate()
    {
        if (firing)
        {
            photonView.RPC("PunDoLaser", PhotonTargets.All, Ship.LocalPlayerInstance.GetPhotonView().viewID, go.GetPhotonView().viewID);
            Units -= Time.deltaTime / 0.5f;
        }
        if (Units <= 0)
            Stop();
    }

    [PunRPC]
    public void PunDoLaser(int parentId, int childId)
    {
        GameObject i = PhotonView.Find(parentId).gameObject;
        GameObject o = PhotonView.Find(childId).gameObject;
      
        Vector3 _endPoint = Vector3.zero;
        RaycastHit2D hit;

        LayerMask layerMask = ~(1 << LayerMask.NameToLayer("Mine") | 1 << LayerMask.NameToLayer("Projectiles"));
        if (hit = Physics2D.Raycast(i.GetComponent<Ship>().PowerUpPosition.transform.position, i.transform.up, 1000f, layerMask))
        {
            _endPoint = hit.point;
            if (hit.collider.GetComponent<Ship>() || hit.collider.transform.root.gameObject.GetComponent<Ship>())
                PhotonView.Find(hit.collider.transform.root.gameObject.GetComponent<PhotonView>().viewID).gameObject.GetComponent<Ship>().TakeDamage(10.0f, i);

            else if (hit.collider.GetComponent<Base>())
                hit.collider.GetComponent<Base>().TakeDamage(10.0f);

            o.GetComponent<UseLaserBeam>()._endPoint = _endPoint;
        }


        o.transform.parent = i.transform;
        o.GetComponent<UseLaserBeam>().Firepoint = i.transform;
        
        o.GetComponent<UseLaserBeam>().LaserPowerUp = this;
        o.GetComponent<UseLaserBeam>().Fire();
    }


    public override void UseContinuousPowerUp()
    {
        go = PhotonNetwork.Instantiate("LaserBeam", Ship.LocalPlayerInstance.GetComponent<Ship>().PowerUpPosition.transform.position, Quaternion.identity, 0);
        firing = true;
    }

    public override void Stop()
    {
        if (go && go.GetComponent<UseLaserBeam>())
        {
            firing = false;
            go.GetComponent<UseLaserBeam>().Stop();
        }
    }
}
