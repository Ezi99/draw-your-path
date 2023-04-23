using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth=100;
    public int currentHealth;
    public HealthBar healthbar;
    void Start()
    {
        currentHealth=maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }
    }
    public void TakeDamage(int dmg)
    {
        currentHealth-=dmg;
        healthbar.SetHealth(currentHealth);
    }

    public void GainHealth(int amountOfHeal)
    {
        currentHealth += amountOfHeal;
        healthbar.SetHealth(currentHealth);
    }
}