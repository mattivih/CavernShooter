using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HUDManager : MonoBehaviour
{


    public bool _zGravityOn = false;
    public bool shieldOn = false;
    private float _zGravityTimer = 15f;
    private float shieldTimer = 15f;
    PowerUp CurrentPU;
    float powerUpFraction;
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    public PhotonPlayer[] players;

  
    void Start()
    {
        //GameObject p3 = player3.gameObject;
        //GameObject p4 = player4.gameObject;

        SetPlayerNames();
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


    public void SetPlayerNames()
    {
        #region enable texts based on the number of players and input playernames based on spawnpoints
        players = PhotonNetwork.playerList;

        if (players.Length == 4)
        {
            foreach (PhotonPlayer p in players)
            {
                if (p.ID == 1)
                    player1.GetComponent<Text>().text = p.NickName;
                if (p.ID == 2)
                    player2.GetComponent<Text>().text = p.NickName;
                if (p.ID == 3)
                    player3.GetComponent<Text>().text = p.NickName;
                if (p.ID == 4)
                    player4.GetComponent<Text>().text = p.NickName;
            }

        }
        else if (players.Length == 3)
        {
            player4.SetActive(false);

            foreach (PhotonPlayer p in players)
            {
                if (p.ID == 1)
                    player1.GetComponent<Text>().text = p.NickName;
                if (p.ID == 2)
                    player2.GetComponent<Text>().text = p.NickName;
                if (p.ID == 3)
                    player3.GetComponent<Text>().text = p.NickName;
            }
        }
        else
        {
            player3.SetActive(false);
            player4.SetActive(false);

            foreach (PhotonPlayer p in players)
            {
                if (p.ID == 1)
                    player1.GetComponent<Text>().text = p.NickName;
                if (p.ID == 2)
                    player2.GetComponent<Text>().text = p.NickName;
            }
        }
        #endregion

        foreach (ShipImageSelector s in GetComponentsInChildren<ShipImageSelector>())
        {
            s.GetInfo();
        }
    }
}
