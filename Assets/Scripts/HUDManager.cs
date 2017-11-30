using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HUDManager : Photon.PunBehaviour
{


    public bool _zGravityOn = false;
    public bool shieldOn = false;
    private float _zGravityTimer = 15f;
    private float shieldTimer = 15f;
    PowerUp CurrentPU;
    float powerUpFraction;
    public Text player1;
    public Text player2;
    public Text player3;
    public Text player4;
    public Image ship1;
    public Image ship2;
    public Image ship3;
    public Image ship4;
    public PhotonPlayer[] players;


    // Use this for initialization
    void Start()
    {
        #region enable texts based on the number of players
        players = PhotonNetwork.playerList;
        Text[] texts = new Text[players.Length];

        if (players.Length == 4)
        {
            player1.text = players[0].NickName;
            player2.text = players[1].NickName;
            player3.text = players[2].NickName;
            player4.text = players[3].NickName;
            texts[0] = player1;
            texts[1] = player2;
            texts[2] = player3;
            texts[3] = player4;

        }
        else if (players.Length == 3)
        {
            player4.GetComponentInChildren<Image>().enabled = false;
            player4.enabled = false;
            player1.text = players[0].NickName;
            player2.text = players[1].NickName;
            player3.text = players[2].NickName;
            texts[0] = player1;
            texts[1] = player2;
            texts[2] = player3;
        }
        else
        {
            player3.GetComponentInChildren<Image>().enabled = false;
            player4.GetComponentInChildren<Image>().enabled = false;
            player3.enabled = false;
            player4.enabled = false;
            player1.text = players[0].NickName;
            player2.text = players[1].NickName;
            texts[0] = player1;
            texts[1] = player2;
        }
        #endregion

        foreach (ShipImageSelector s in GetComponentsInChildren<ShipImageSelector>())
        {
            s.GetInfo();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Player && GameManager.Instance.Player.GetPhotonView().isMine)
        {
            if (CurrentPU)
            {
                powerUpFraction = CurrentPU.Units / CurrentPU.MaxUnits;
                GameManager.Instance.powerupBarImage.fillAmount = powerUpFraction;
            }


            if (GameManager.Instance.healthbarImage)
            {
                float healthFraction = GameManager.Instance.Player.GetComponent<Ship>().Health / GameManager.Instance.Player.GetComponent<Ship>().MaxHealth;
                healthFraction *= 0.92f;
                healthFraction += 0.04f;
                GameManager.Instance.healthbarImage.fillAmount = healthFraction;
                if (healthFraction > 0.5)
                {
                    GameManager.Instance.healthbarImage.color = Color.Lerp(Color.yellow, Color.green, (healthFraction - 0.5f) * 2);
                }
                else
                {
                    GameManager.Instance.healthbarImage.color = Color.Lerp(Color.red, Color.yellow, healthFraction * 2);
                }
            }


            if (_zGravityOn)
            {
                _zGravityTimer -= Time.deltaTime;
                GameManager.Instance.powerupBarImage.fillAmount = (_zGravityTimer / 15f);

            }
            if (_zGravityTimer <= 0f)
                _zGravityOn = false;

            if (shieldOn)
            {
                shieldTimer -= Time.deltaTime;
                GameManager.Instance.powerupBarImage.fillAmount = (shieldTimer / 15f);

            }
            if (_zGravityTimer <= 0f)
                _zGravityOn = false;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void UpdatePowerUp(PowerUp CurrentPowerUp)
    {
        CurrentPU = CurrentPowerUp;
        if (CurrentPowerUp.name != "ZeroGravityPowerUp")
            _zGravityOn = false;
        if (CurrentPowerUp.name != "ShieldPowerUp")
            shieldOn = false;
        else
        {
            GameManager.Instance.powerupBarImage.fillAmount = 1f;
            return;
        }



        if (CurrentPowerUp.Units == 0)
        {
            GameManager.Instance.powerupBarLines.enabled = false;
            GameManager.Instance.powerupBarLines4.enabled = false;
        }

        else if (CurrentPowerUp.MaxUnits == 3 && CurrentPowerUp.Units != 0)
        {
            GameManager.Instance.powerupBarLines4.enabled = false;
            GameManager.Instance.powerupBarLines.enabled = true;
        }
        else if (CurrentPowerUp.MaxUnits == 4 && CurrentPowerUp.Units != 0)
        {
            GameManager.Instance.powerupBarLines.enabled = false;
            GameManager.Instance.powerupBarLines4.enabled = true;
        }

        else
        {
            GameManager.Instance.powerupBarLines.enabled = false;
            GameManager.Instance.powerupBarLines4.enabled = false;
        }

    }

    public void ResetZeroGravity()
    {
        GameManager.Instance.powerupBarLines.enabled = false;
        GameManager.Instance.powerupBarLines4.enabled = false;
        _zGravityOn = true;
        _zGravityTimer = 15f;

    }

    public void ResetShield()
    {
        GameManager.Instance.powerupBarLines.enabled = false;
        GameManager.Instance.powerupBarLines4.enabled = false;
        shieldOn = true;
        shieldTimer = 15f;
    }

    public void UpdateHealthBar(float health, float maxHealth)
    {
        float healthFraction = (health / maxHealth);
        healthFraction *= 0.92f;
        healthFraction += 0.04f;
        GameManager.Instance.healthbarImage.fillAmount = healthFraction;
        if (healthFraction > 0.5)
        {
            GameManager.Instance.healthbarImage.color = Color.Lerp(Color.yellow, Color.green, (healthFraction - 0.5f) * 2);
        }
        else
        {
            GameManager.Instance.healthbarImage.color = Color.Lerp(Color.red, Color.yellow, healthFraction * 2);
        }

    }
}
