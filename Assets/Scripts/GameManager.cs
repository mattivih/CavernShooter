using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class GameManager : Photon.PunBehaviour {

    //Register an active player game object in the scene
    public GameObject Player;
    public GameObject Background;
    public enum powerupPickups
    {
        DistortionRayPowerUp = 0,
        FlamethrowerPowerUp = 1,
        HealthPowerup = 2,
        LaserPowerUp = 3,
        MinePowerUp = 4,
        ShieldPowerup = 5,
        TorpedoPowerup = 6,
        ZeroGravityPowerup = 7
    }
    public bool spectating;
    public Image healthbarImage;
    public Image powerupBarImage;
    public Image powerupBarLines;
    public Image powerupBarLines4;
    public Image shieldbarImage;
    public Image powerupImage;
    public GameObject GameOverPrefab;
    public HUDManager hud;
    public destroyParticleSystem destroyParticleSystem;

    public static GameManager Instance = null;
    public static Level02SpriteManager SpriteManager = null;

    private List<GameObject> shipList;
    private int spectateIndex;



    
    /// <summary>
    /// Ensures there's only one GameManager
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
            SpriteManager = GetComponent<Level02SpriteManager>();
            Instance.shipList = new List<GameObject>();
        } else if (Instance != this) {
            DestroyImmediate(gameObject);
        }
    }

    void Update()
    {
        if (Instance.spectating)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Instance.spectateIndex == (shipList.Count - 1))
                {
                    for (int i = 0; i < shipList.Count; i++)
                    {
                        if (shipList[i] != null)
                        {
                            Instance.Player = shipList[i];
                            Instance.spectateIndex = i;
                            Camera.main.GetComponent<CameraController>().FollowShip(Instance.Player.transform);
                            break;
                        }
                    }
                    
                }
                else
                {
                    for (int i = 0; i < shipList.Count; i++)
                    {
                        if (i > spectateIndex) 
                        {
                            if (shipList[i] != null)
                            {
                                Instance.Player = shipList[i];
                                Camera.main.GetComponent<CameraController>().FollowShip(Instance.Player.transform);
                                Instance.spectateIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public void SpectateSpecific(GameObject ship, int shipNumber)
    {
        Instance.spectateIndex = shipNumber;
        Instance.Player = ship;
        Camera.main.GetComponent<CameraController>().FollowShip(ship.transform);
    }

    public void SpectateFirst()
    {
        Instance.Player = shipList[0];
        Camera.main.GetComponent<CameraController>().FollowShip(Instance.Player.transform);
    }

    public int AddShipToList(GameObject ship)
    {
        int shipNumber = shipList.Count;
        Instance.shipList.Insert(shipNumber, ship);
        return shipNumber;
    }

    public void RemoveShipFromList(int shipNumber)
    {
        Instance.shipList.RemoveAt(shipNumber);
    }

    void Start()
    {

        hud = FindObjectOfType<HUDManager>();
    }

    public void UpdateHealthBar(float health, float maxHealth)
    {
        hud.UpdateHealthBar(health, maxHealth);
    }


    public void UpdatePowerUp(PowerUp CurrentPowerUp) {
        hud.UpdatePowerUp(CurrentPowerUp);
    }
}
