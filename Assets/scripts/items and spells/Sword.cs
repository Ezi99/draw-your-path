using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private int damage=34;
    private int durability;
    private bool canDamage = true;
    private float damageCooldown = 1f; // Adjust the cooldown duration as needed

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
        if (canDamage && other.CompareTag("Erika"))
        {
            Debug.Log("Erika");
            ErikaScript erika = other.GetComponentInParent<ErikaScript>();
            if (erika != null)
            {
                // Deal damage to Erika
                Debug.Log("stabbed");
                erika.takeDamage(damage);
                canDamage = false;
                Invoke("ResetDamageCooldown", damageCooldown);
            }
        }
        else if (canDamage && other.CompareTag("Paladin"))
        {
            Debug.Log("Paladin");
            // Check if the collided object has a Paladin script component
            PaladinScript paladin = other.GetComponentInParent<PaladinScript>();

            if (paladin != null)
            {
                // Deal damage to Paladin
                Debug.Log("stabbed");
                paladin.takeDamage(damage);
                canDamage = false;
                Invoke("ResetDamageCooldown", damageCooldown);
            }
        }
        else if (canDamage && other.CompareTag("Paladin_Shield"))
        {
            Debug.Log("Blocked");
            canDamage = false;
            Invoke("ResetDamageCooldown", damageCooldown);
        }


        

    }
    private void ResetDamageCooldown()
    {
        canDamage = true;
    }
}