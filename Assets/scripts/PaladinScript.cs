using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinScript : MonoBehaviour
{
    public EnemySwordAttack attack;
    int health = 100;
    public void enableDmg()
    {
        attack.dealDmg();
    }
    public void takeDamage(int dmg)
    {
        Debug.Log($"Paladin Health -{dmg}");
        health -= dmg;
        Debug.Log($"current health {health}");
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
