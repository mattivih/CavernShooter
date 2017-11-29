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
    public Image healthbarImage;
    public Image powerupBarImage;
    public Image powerupBarLines;
    public Image powerupBarLines4;
    public Image shieldbarImage;
    public Image powerupImage;
    public GameObject GameOverPrefab;
    public HUDManager hud;
    public destroyParticleSystem destroyParticleSystem;


    //Refactored to use Photon Actor ID:s
    //public List<NetworkInstanceId> players = new List<NetworkInstanceId>();
    //public List<int> players = new List<int>();
    

    public static GameManager Instance = null;
    public static Level02SpriteManager SpriteManager = null;
    
    /// <summary>
    /// Ensures there's only one GameManager
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
            SpriteManager = GetComponent<Level02SpriteManager>();
        } else if (Instance != this) {
            DestroyImmediate(gameObject);
        }
    }


    void Start()
    {
        hud = FindObjectOfType<HUDManager>();
    }

    public void UpdateHealthBar(float health, float maxHealth)
    {
        if (hud) {
            hud.UpdateHealthBar(health, maxHealth);
        }
    }

    public void UpdateShieldBar(float shield, float maxHealth)
    {
        if (hud) {
            hud.UpdateShieldBar(shield, maxHealth);
        }
    }

    public void UpdatePowerUp(PowerUp CurrentPowerUp) {
        if (hud) {
            hud.UpdatePowerUp(CurrentPowerUp);
        }
    }
}
