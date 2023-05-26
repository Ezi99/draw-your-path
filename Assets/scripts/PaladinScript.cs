using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinScript : MonoBehaviour
{
    private Animator animator;
    public EnemySwordAttack attack;
    int health = 100;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void enableDmg()
    {
        attack.dealDmg();
    }
    public void takeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log($"Paladin Health -{dmg} current health {health}");
        if (health <= 0)
        {
            die();
        }
    }
    private void die()
    {
        Debug.Log("Death animation triggered");
        animator.SetBool("Death", true);
    }
    public void destroy()
    {
        Destroy(gameObject);
    }
}
