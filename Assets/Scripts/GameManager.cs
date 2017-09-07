using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Threading;

public class GameManager : NetworkBehaviour {

    //Register an active player game object in the scene
    public GameObject Player;
    public GameObject Background;
    public Sprite[] powerupImages;
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
    public GameObject GameOverPrefab;
    public HUDManager hud;

    public List<NetworkInstanceId> players = new List<NetworkInstanceId>();

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
        hud = FindObjectOfType<HUDManager> ();
    }
    public void UpdateHealthBar(float health, float maxHealth)
    {
        hud.UpdateHealthBar(health, maxHealth);
    }

    public void UpdateShieldBar(float shield, float maxHealth)
    {
        hud.UpdateShieldBar(shield, maxHealth);
    }

    public void UpdatePowerUp() {
        hud.UpdatePowerUp(Player.GetComponent<PowerUpHandler>().CurrentPowerUp);
    }

    public int GetPlayerNum(NetworkInstanceId playerid) {
        if (!players.Contains(playerid)) {
            players.Add(playerid);
        }
        List<NetworkInstanceId> sortedList = players.OrderBy(o => o.Value).ToList();
        return sortedList.IndexOf(playerid);
    }

    public void CallEndGame(string[] deadPlayers) {
        RpcShowMatchResult(deadPlayers);
    }


    [ClientRpc]
    public void RpcShowMatchResult(string[] deadPlayers)
    {
        GameObject gameOverScreen = Instantiate(GameOverPrefab);
        MatchResultList result = gameOverScreen.GetComponentInChildren<MatchResultList>();
        for (int i = deadPlayers.Length - 1; i >= 0; i--)
        {
            result.FillPlayerInfo(deadPlayers[i]);
        }
    }
}
