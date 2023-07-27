using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ErikaCombat : MonoBehaviour
{
    public Animator animator;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
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
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
        Invoke("ResetAttackFlag", 0.5f);
    }

    void ResetAttackFlag()
    {
        isAttacking = false;
    }
}
