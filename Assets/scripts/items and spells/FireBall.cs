using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private int damage=100;

    private void Start()
    {
        Destroy(gameObject, 5);
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has an Erika script component
        if (other.CompareTag("Erika"))
        {
            Debug.Log("Erika");
            ErikaScript erika = other.GetComponentInParent<ErikaScript>();
            if (erika != null)
            {
                // Deal damage to Erika
                Debug.Log("NUKED");
                erika.takeDamage(damage);
            }
            else if (other.CompareTag("Paladin"))
            {
                Debug.Log("Paladin");
                // Check if the collided object has a Paladin script component
                PaladinScript paladin = other.GetComponentInParent<PaladinScript>();

                if (paladin != null)
                {
                    // Deal damage to Paladin
                    Debug.Log("NUKED");
                    paladin.takeDamage(damage);
                }
            }




        }
    }
    public void SetStats(int dmg)
    {
        damage = dmg;
    }
}
