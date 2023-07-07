using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthbar;
    public AreaNavigator m_AreaNavigator;
    public GameObject m_HealthBarPic;
    private bool m_beenHit = false;

    void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int dmg)
    {
        Debug.Log($"Plyer HP -{dmg}");
        currentHealth -= dmg;
        healthbar.SetHealth(currentHealth);
        ShowHealthBarPic();
        if (currentHealth < 0)
        {
            m_AreaNavigator.ReSpawnPlayer();
            currentHealth = maxHealth;
            healthbar.SetHealth(currentHealth);
            m_HealthBarPic.SetActive(true);
            Invoke("HideHealthBarPic", 5);
        }

    }

    public void GainHealth(int amountOfHeal)
    {
        currentHealth += amountOfHeal;
        healthbar.SetHealth(currentHealth);
    }

    private void ShowHealthBarPic()
    {
        if (m_beenHit == false)
        {
            m_beenHit = true;
            m_HealthBarPic.SetActive(true);
            Invoke("HideHealthBarPic", 5);
        }
    }

    private void HideHealthBarPic()
    {
        m_HealthBarPic.SetActive(false);
        m_beenHit = false;
    }
}