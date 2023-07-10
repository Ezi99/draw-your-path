using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class PaladinScript : MonoBehaviour
{
    public GameObject enemyCanvas;
    private Animator animator;
    public EnemySwordAttack attack;
    private int health = 100;
    public Slider healthBar;
    private bool died=false;

    private void Update()
    {
        healthBar.value = health;
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void enableDmg()
    {
        attack.DealDamage();
    }

    public void takeDamage(int dmg,bool head)
    {
        health -= dmg;
        Debug.Log($"Paladin Health - {dmg} current health {health}");
        if (!died && health <= 0)
        {
            Die();
        }
        if (!died && head)
        {
            Debug.Log("HEAAAAAADDDDD");
            animator.SetTrigger("HeadShot");
        }
        else
        {
            Debug.Log("BOOOOODDDDDYYY");
            animator.SetTrigger("BodyShot");
        }
     
    }

    private void Die()
    {
        died = true;
        enemyCanvas.SetActive(false);
        Debug.Log("Death animation triggered");
        animator.SetTrigger("Death");
    }

    public void destroy()
    {
        Destroy(gameObject);
    }
}
