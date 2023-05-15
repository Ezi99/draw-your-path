using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
  public Animator animator;
  private UnityEngine.AI.NavMeshAgent navMeshAgent;
    int count=0;
    // Set stopping range to 1.5
    public float stoppingRange = 1.5f;
    public float attackCooldown = 2f;
    private bool isAttacking = false; 
    private float lastAttackTime; 
    Transform target;
    void Start()
    {
        target=PlayerManager.instance.player.transform;
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (distanceToPlayer <= stoppingRange && !isAttacking)
        {
            // The enemy has reached the stopping point
            Attack();
            // Add your code here
        }
        
    }
    void Attack()
    {
        // Set the flag to prevent multiple attacks in quick succession.
        isAttacking = true;
        
        // Check if enough time has passed since the last attack.
        if (Time.time - lastAttackTime > attackCooldown)
        {
            
            // Apply damage to the player.
            //player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            
            if(count<=2)
            {
            animator.SetTrigger("Attack");
            }
            else
            {
            animator.SetTrigger("Kick");
            count=0;
            }

            count++;
            // Update the time of the last attack.
            lastAttackTime = Time.time;
        }
        
        // Reset the flag after a short delay.
        Invoke("ResetAttackFlag", 0.5f);
    }
    
    void ResetAttackFlag()
    {
        isAttacking = false;
    }
}
