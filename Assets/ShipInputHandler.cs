using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInputHandler : MonoBehaviour {

    public Transform Mesh;
    public GameObject[] Firepoints;
    public GameObject LaserPrefab;
    public float Rotation, MaxRotation = 30f, Speed, FireRate = 0.1f, MaxSpeed, ProjectileSpeed = 50f;

    private Vector3 originalMeshRotation;
    private Thruster _thruster;
    private Rigidbody2D _rigid;
    private int _laserCounter;

    private float _accelDrag = 1, _freeDrag = 0, _timer = 0.5f;

	// Update is called once per frame
	void Update () {

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (vertical > 0)
        {
            Move(vertical);
            _rigid.drag = _accelDrag;
            //thrusteron
            if (_thruster)
            {
                _thruster.ThrusterOn();
                //CmdThrusterOn();
            }
        }
        else
        {
            GetComponent<Rigidbody2D>().drag = _freeDrag;
            //thrusteroff
            if (_thruster)
            {
                _thruster.ThrusterOff();
                //CmdThrusterOff();
            }
        }

        if (horizontal != 0)
        {
            transform.Rotate(Vector3.forward * Rotation * -horizontal * Time.deltaTime);
            //_rigid.AddTorque(Rotation * -horizontal, ForceMode2D.Force);
            Vector3 bank = originalMeshRotation;
            bank.y += Mathf.Sign(horizontal) * -MaxRotation;
            Mesh.localRotation = Quaternion.Lerp(Mesh.localRotation, Quaternion.Euler(bank), Time.deltaTime * (Rotation / 150));
        }
        else
        {
            Vector3 bankNone = originalMeshRotation;
            Mesh.localRotation = Quaternion.Lerp(Mesh.localRotation, Quaternion.Euler(bankNone), Time.deltaTime * (Rotation / 120));
        }
        if (Input.GetKey(KeyCode.Period))
        {
            if (_timer > FireRate)
            {
                Fire();
                _timer = 0;
            }
            _timer += Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.Period))
        {
            _timer = FireRate;
        }
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            //GetComponent<PowerUpHandler>().Use();
        }
        if (Input.GetKeyUp(KeyCode.Comma))
        {
            //GetComponent<PowerUpHandler>().Stop();
        }
    }

    /// <summary>
    /// Moves the ship.
    /// </summary>
    void Move(float mult)
    {
        float vel = transform.InverseTransformDirection(GetComponent<Rigidbody2D>().velocity).sqrMagnitude;
        //Debug.Log(vel);
        if (vel < MaxSpeed)
        {
            _rigid.AddForce(transform.up * Speed * mult, ForceMode2D.Force);
        }
    }


    //TODO: refactor color code
    void Fire()
    {
        foreach (GameObject t in Firepoints)
        {
            GameObject laser = Instantiate(LaserPrefab, t.transform.position, t.transform.rotation);
            laser.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
            laser.GetComponent<Rigidbody2D>().AddForce(transform.up * ProjectileSpeed, ForceMode2D.Impulse);
            laser.name = "Clientlaser " + _laserCounter;
            laser.GetComponent<Laser>().clientSide = true;
            //laser.GetComponent<Laser>().myColor = shipColors[playerNum];
            laser.GetComponent<Collider2D>().enabled = false;
            //Color color = shipColors[playerNum];
            //CmdFire(GetComponent<NetworkIdentity>().netId, t.transform.position, t.transform.rotation, laser.GetComponent<Rigidbody2D>().velocity, laserCounter, color);
            _laserCounter++;
        }
    }
}
