using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paladinLeg : MonoBehaviour
{
    public float m_DamageCooldown = 1f; // Adjust the cooldown duration as needed
    private bool m_CanDamage = false;
    private int m_AmountOfDamageToDeal = 10;


    public void enableDmg()
    {
        m_CanDamage = true;
    }

    private void OnTriggerEnter(Collider other)
    {
      
        if (m_CanDamage)
        
            
            if (other.CompareTag("Shield"))
            {
                Debug.Log("Easy Block");
                other.gameObject.GetComponent<Shield>().TakeDamage(10);
                m_CanDamage = false;
                //Invoke("ResetDamageCooldown", m_DamageCooldown);
            }
            else if (other.CompareTag("Player"))
            {
                Debug.Log("ayyyy");
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(m_AmountOfDamageToDeal);
                m_CanDamage = false;
            }
        }

    }


