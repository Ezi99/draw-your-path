using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinScript : MonoBehaviour
{
    int health = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Destroy(gameObject);
    }
}
