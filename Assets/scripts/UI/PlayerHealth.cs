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
    public GameObject m_WeakRegainPic;
    public GameObject m_RegularRegainPic;
    public GameObject m_StrongRegainPic;
    private bool m_ShowingHealth = false;
    private const int m_HideDuration = 2;

    void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int dmg)
    {
        Debug.Log($"Player HP -{dmg}");
        currentHealth -= dmg;
        healthbar.SetHealth(currentHealth);
        ShowHealthBarPic();
        if (currentHealth <= 0)
        {
            m_AreaNavigator.ReSpawnPlayer();
            currentHealth = maxHealth;
            healthbar.SetHealth(currentHealth);
            m_HealthBarPic.SetActive(true);
            Invoke("HideHealthBarPic", m_HideDuration);
        }

    }

    public void GainHealth(int amountOfHeal)
    {
        currentHealth += amountOfHeal;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthbar.SetHealth(currentHealth);
        ShowHealthBarPic();
    }

    private void ShowHealthBarPic()
    {
        if (m_ShowingHealth == false)
        {
            m_ShowingHealth = true;
            m_HealthBarPic.SetActive(true);
            Invoke("HideHealthBarPic", m_HideDuration);
        }
    }

    private void HideHealthBarPic()
    {
        m_WeakRegainPic.SetActive(false);
        m_RegularRegainPic.SetActive(false);
        m_StrongRegainPic.SetActive(false);
        m_HealthBarPic.SetActive(false);
        m_ShowingHealth = false;
    }
}