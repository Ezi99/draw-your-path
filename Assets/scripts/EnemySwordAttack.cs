using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordAttack : MonoBehaviour
{
    bool dealDamage;
    public void dealDmg()
    {
        Debug.Log("dmg enabled");
        dealDamage = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (dealDamage)
            {
                Debug.Log("ayyyy");
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(10);
                dealDamage = false;
            }
            
        }
    }
}
