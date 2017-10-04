using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HUDManager : NetworkBehaviour {

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdatePowerUp(GameObject CurrentPowerUp) {
        if (GameManager.Instance.powerupBarImage)
        {
            if (CurrentPowerUp)
            {
                float powerUpFraction = CurrentPowerUp.GetComponent<PowerUp>().Units / CurrentPowerUp.GetComponent<PowerUp>().MaxUnits;
                GameManager.Instance.powerupBarImage.fillAmount = powerUpFraction;
            
                if (CurrentPowerUp.GetComponent<PowerUp>().MaxUnits == 3)
                {
                    GameManager.Instance.powerupBarLines4.enabled = false;
                    GameManager.Instance.powerupBarLines.enabled = true;          
                }
                else if (CurrentPowerUp.GetComponent<PowerUp>().MaxUnits == 4)
                {
                    GameManager.Instance.powerupBarLines.enabled = false;
                    GameManager.Instance.powerupBarLines4.enabled = true;                 
                }
           
                else if(CurrentPowerUp.GetComponent<PowerUp>().MaxUnits != 4 && CurrentPowerUp.GetComponent<PowerUp>().MaxUnits != 3)
                {
                    GameManager.Instance.powerupBarLines.enabled = false;
                    GameManager.Instance.powerupBarLines4.enabled = false;
                }
                 
            }
        }
    }

    public void UpdateHealthBar(float health, float maxHealth)
    {
        /*if (GameManager.Instance.healthbarImage)
        {*/
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
        /*}*/

    }

    public void UpdateShieldBar(float shield, float maxHealth)
    {
        /*if (GameManager.Instance.shieldbarImage)
        {*/
        GameManager.Instance.shieldbarImage.fillAmount = shield / maxHealth;
        /*}*/
    }


}
