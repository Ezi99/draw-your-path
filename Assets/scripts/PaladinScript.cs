using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinScript : MonoBehaviour
{
    private Animator animator;
    public EnemySwordAttack attack;
    private int health = 100;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void enableDmg()
    {
        attack.DealDamage();
    }

    public void takeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log($"Paladin Health - {dmg} current health {health}");
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Death animation triggered");
        animator.SetTrigger("Death");
    }

    public void destroy()
    {
        Destroy(gameObject);
    }
}
