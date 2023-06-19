using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PaladinCombat : MonoBehaviour
{
    public Animator animator;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private int count = 0;
    
    public float stoppingRange = 1.5f;
    public float attackCooldown = 2f;
    private bool isAttacking = false;
    private float lastAttackTime;
    Transform target;
    System.Random random = new System.Random();

    void Start()
    {
        target = PlayerManager.instance.player.transform;
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
        
        isAttacking = true;

        
        if (Time.time - lastAttackTime > attackCooldown)
        {
            int TypeOfAttack = random.Next(1, 4);
            // Apply damage to the player.
            //player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);

            if (TypeOfAttack == 1)
            {
                animator.SetTrigger("Attack");
            }
            else if(TypeOfAttack == 2)
            {
                animator.SetTrigger("Kick");
                count = 0;
            }
            else if(TypeOfAttack ==3)
            {
                animator.SetTrigger("Block");
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
