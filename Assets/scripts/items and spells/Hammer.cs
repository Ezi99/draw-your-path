using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hammer : MonoBehaviour
{
    private int damage=50;
    private int durability;
    private bool canDamage = true;
    private float damageCooldown = 1f; // Adjust the cooldown duration as needed
    private Vector3 prevPosition;
    private Vector3 velocity;
    private float prevTime;


    private void Start()
    {
        prevPosition = transform.position;
        prevTime = Time.time;
    }

    private void Update()
    {
        velocity = (transform.position - prevPosition) / (Time.time - prevTime);
        prevPosition = transform.position;
        prevTime = Time.time;
    }

    public void SetStats(int dmg, int Durability)
    {
        damage = dmg;
        durability = Durability;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canDamage && other.CompareTag("Erika"))
        {
            Debug.Log("Erika");
            ErikaScript erika = other.GetComponentInParent<ErikaScript>();
            if (erika != null)
            {
                // Deal damage to Erika
                Debug.Log("hammered");
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
                Debug.Log("hammered");
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
        else if (other.CompareTag("destructible wall") == true)
        {
            DestructibleWall wall = other.GetComponent<DestructibleWall>();
            if (wall != null)
            {
                wall.TakeDamage(25, (int)velocity.magnitude);
            }
        }
        else if (other.CompareTag("Gate") == true)
        {
            VillageGate gate = other.GetComponent<VillageGate>();
            if (gate != null)
            {
                gate.TakeDamage((int)velocity.magnitude);
            }
        }

    }
    private void ResetDamageCooldown()
    {
        canDamage = true;
    }
}
