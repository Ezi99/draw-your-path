using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private int damage=20;
    private int durability;

    void Update()
    {

    }

    public void SetStats(int dmg, int Durability)
    {
        damage = dmg;
        durability = Durability;
    }

     private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has an Erika script component
        ErikaScript erika = other.GetComponent<ErikaScript>();
        
        if (erika != null)
        {
            // Deal damage to Erika
            Debug.Log("stabbed");
            erika.takeDamage(damage);
            return; // Exit the method to avoid calling other TakeDamage methods
        }

        // Check if the collided object has a Paladin script component
        PaladinScript paladin = other.GetComponent<PaladinScript>();

        if (paladin != null)
        {
            // Deal damage to Paladin
            paladin.takeDamage(damage);
        }
    }
}