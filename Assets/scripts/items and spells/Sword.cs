using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private int damage=34;
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
        if (other.CompareTag("Erika"))
        {
            ErikaScript erika = other.GetComponent<ErikaScript>();
            if (erika != null)
            {
                // Deal damage to Erika
                Debug.Log("stabbed");
                erika.takeDamage(damage);
                return; 
            }
        }
        else if (other.CompareTag("Paladin"))
        {
            // Check if the collided object has a Paladin script component
            PaladinScript paladin = other.GetComponent<PaladinScript>();

            if (paladin != null)
            {
                // Deal damage to Paladin
                Debug.Log("stabbed");
                paladin.takeDamage(damage);
            }
        }
        
        

        
    }
}