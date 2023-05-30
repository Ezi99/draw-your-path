using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordAttack : MonoBehaviour
{
    bool dealDamage;
    private bool canDamage = true;
    private float damageCooldown = 1f; // Adjust the cooldown duration as needed
    public void dealDmg()
    {
        Debug.Log("dmg enabled");
        dealDamage = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (canDamage && other.CompareTag("Player"))
        {
            if (dealDamage)
            {
                Debug.Log("ayyyy");
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(10);
                dealDamage = false;
            }
        }
        else if (other.CompareTag("Shield"))
        {
            Debug.Log("Easy Block");
            canDamage = false;
            Invoke("ResetDamageCooldown", damageCooldown);
        }
    }
}

