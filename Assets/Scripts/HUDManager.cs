using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HUDManager : MonoBehaviour {

    public bool _zGravityOn = false;
    private float _zGravityTimer = 15f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (_zGravityOn)
        {
            _zGravityTimer -= Time.deltaTime;
            GameManager.Instance.powerupBarImage.fillAmount = (_zGravityTimer / 15f);
            
        }
        if (_zGravityTimer <= 0f)
            _zGravityOn = false;
    }

    public void UpdatePowerUp(PowerUp CurrentPowerUp) {
        if (CurrentPowerUp.name != "ZeroGravityPowerUp")
            _zGravityOn = false;
        else
        {
            GameManager.Instance.powerupBarImage.fillAmount = 1f;
            return;
        }


        float powerUpFraction = CurrentPowerUp.Units / CurrentPowerUp.MaxUnits;
        GameManager.Instance.powerupBarImage.fillAmount = powerUpFraction;

        if (CurrentPowerUp.MaxUnits == 3 && CurrentPowerUp.Units != 0)
        {
            GameManager.Instance.powerupBarLines4.enabled = false;
            GameManager.Instance.powerupBarLines.enabled = true;
        }
        else if (CurrentPowerUp.MaxUnits == 4)
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

    public void UpdateShieldBar(float shield, float maxHealth)
    {
        /*if (GameManager.Instance.shieldbarImage)
        {*/
        GameManager.Instance.shieldbarImage.fillAmount = shield / maxHealth;
        /*}*/
    }


}
