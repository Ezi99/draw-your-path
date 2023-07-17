using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordAttack : MonoBehaviour
{
    public float m_DamageCooldown = 1f; // Adjust the cooldown duration as needed

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
        if (other.CompareTag("Shield"))
        {
            Debug.Log("Easy Block");
            other.gameObject.GetComponent<Shield>().TakeDamage(10);
            m_CanDamage = false;
            Invoke("ResetDamageCooldown", m_DamageCooldown);
        }
        else if (m_CanDamage && other.CompareTag("Player"))
        {
            if (m_DealDamage)
            {
                Debug.Log("ayyyy");
                m_SwordHitSound.Play();
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(m_AmountOfDamageToDeal);
                m_DealDamage = false;
            }
        }
        
    }
}

