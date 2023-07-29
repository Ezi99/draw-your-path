using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordAttack : MonoBehaviour
{
    public float m_DamageCooldown = 1f; // cooldown duration 
    private bool m_DealDamage;
    private bool m_CanDamage = true;
    private AudioSource m_SwordHitSound;
    private int m_AmountOfDamageToDeal = 10;

    private void Start()
    {
        m_SwordHitSound = GetComponent<AudioSource>();
    }

    public void DealDamage()
    {
        Debug.Log("dmg enabled");
        m_DealDamage = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_CanDamage) // in order to not deal damage to both shield and player multiple times in single attack
        {
            if (m_DealDamage && other.CompareTag("Shield"))
            {
                Debug.Log("Player Blocked");
                other.gameObject.GetComponent<Shield>().TakeDamage(m_AmountOfDamageToDeal);
                m_CanDamage = false;
                Invoke("ResetDamageCooldown", m_DamageCooldown);
            }
            else if (m_DealDamage && other.CompareTag("Player"))
            {

                Debug.Log("Palyer slashed");
                m_SwordHitSound.Play();
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(m_AmountOfDamageToDeal);
                m_DealDamage = false;
                m_CanDamage = false;
                Invoke("ResetDamageCooldown", m_DamageCooldown);
            }
        }

    }
    private void ResetDamageCooldown()
    {
        m_CanDamage = true;
    }
}

