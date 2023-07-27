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
            Attack();
        }

    }
    void Attack()
    {
        
        isAttacking = true;

        
        if (Time.time - lastAttackTime > attackCooldown)
        {
            int TypeOfAttack = random.Next(1, 4);
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
            lastAttackTime = Time.time;
        }
        Invoke("ResetAttackFlag", 0.5f);
    }

    void ResetAttackFlag()
    {
        isAttacking = false;
    }

}
