using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UseTorpedo : Photon.PunBehaviour {

	public float Speed = 15f;
	public float TurningSpeedMultiplier = 0.05f;
    public float TurningSpeedAngular;
    public float zRotationSpeed;
	public float TorpedoLifetime = 2f;
    float maxAngle = 0.01f;
	public ParticleSystem EmitFire, TorpedoExplosionPrefab;
	public GameObject source = null;
    string[] tags;
    List<GameObject> targetObjects = new List<GameObject>();
    private GameObject t;

    void Start()
    {
        
        tags = new string[] { "Enemy", "Player" };
        foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (tags.Contains(go.tag))
                targetObjects.Add(go);
            if (go == source)
                targetObjects.Remove(go);
        }
        Debug.Log(source);
    }

    /// <summary>
    /// Locks on to the enemy player and starts flying torwards it at a given speed and turning speed.
    /// </summary>
    void Update()
    {
        // GameObject[] targetObjects = GameObject.FindGameObjectsWithTag("Enemy");

        if (targetObjects.Count==0) {
			Vector2 moveDirection = GetComponent<Rigidbody2D>().velocity;
			float angle = 0f;
			if (moveDirection != Vector2.zero) {
				angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
			}
			Quaternion newRot = Quaternion.AngleAxis(angle, Vector3.forward);
			if (transform.rotation != newRot) {
				transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.deltaTime * TurningSpeedMultiplier*0.3f);
			}

		} else {
			float closest = float.MaxValue;
			GameObject closestTarget=null;
			for(int i=0;i<targetObjects.Count;i++) {
                if (targetObjects[i] == null)
                    return;
				float dist = Vector3.Distance(targetObjects[i].transform.position, transform.position);
				if (dist < closest) {
					closest = dist;
					closestTarget = targetObjects[i];
				}
			}
			Vector3 Target = closestTarget.transform.position;
			Vector3 Distance = Target - transform.position;

			Vector2 lookDir = transform.up;

			Vector3 cross = Vector3.Cross(Distance, lookDir);
			// actually get the sign (either 1 or -1)
			float sign = Mathf.Sign(cross.z);

			// the angle, ranging from 0 to 180 degrees
			float angle = Vector2.Angle(Distance, lookDir);

			float turn = TurningSpeedMultiplier * Mathf.Sqrt(Mathf.Abs(angle)/ TurningSpeedAngular);
			// apply the sign to get angles ranging from -180 to 0 to +180 degrees
			angle *= sign;
			// apply torque in the opposite direction to decrease angle
			if (Mathf.Abs(angle) > maxAngle) {
				GetComponent<Rigidbody2D>().AddTorque(-sign * turn * Time.deltaTime);
			}
		}

		Vector3 meshRot = GetComponentInChildren<MeshRenderer>().gameObject.transform.eulerAngles;
		meshRot.z += Time.deltaTime * zRotationSpeed;
		GetComponentInChildren<MeshRenderer>().gameObject.transform.eulerAngles = meshRot;

		GetComponent<Rigidbody2D>().AddForce(transform.up * Speed *Time.deltaTime * 30, ForceMode2D.Force);
	}

    /// <summary>
    /// Torpedo detonates when it comes close enough to an enemy player or terrain.
    /// </summary>
    /// <param name="collider">Collider.</param>
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject == source)
            return;

        photonView.RPC("disableTorpedo", PhotonTargets.All, photonView.viewID, collision.gameObject.GetComponent<PhotonView>().viewID);
        Invoke("photonDestroyExplosion", 2.0f);

        if(gameObject.GetPhotonView().isMine)
            t = PhotonNetwork.Instantiate("TorpedoExplosion", collision.otherCollider.transform.position, collision.transform.rotation, 0);

        EmitFire.emissionRate = 0;

        // This splits the particle off so it doesn't get deleted with the parent

        // this stops the particle from creating more bits


    }
    public void photonDestroyExplosion()
    {
        if(gameObject.GetPhotonView().isMine)
        {
            PhotonNetwork.Destroy(gameObject);
            PhotonNetwork.Destroy(t.gameObject);           
        }
       
    }

    [PunRPC]
    public void disableTorpedo(int objectId, int viewId)
    {
        PhotonView.Find(objectId).gameObject.SetActive(false);
        if (PhotonView.Find(viewId).gameObject.GetComponent<Ship>() && PhotonView.Find(viewId).gameObject != source)
            PhotonView.Find(viewId).gameObject.GetComponent<Ship>().TakeDamage(100f, source);
    }
}