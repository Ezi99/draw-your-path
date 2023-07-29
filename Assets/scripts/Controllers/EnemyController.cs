using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    public float lookRadius= 10f;

    Transform target;
    NavMeshAgent agent;
    bool isMoving;
    private Animator animator;
    private bool isAttacking;
    private Vector3 m_PrevPosition;
    private Vector3 m_Velocity;
    private float m_PrevTime;


    void Start()
    {
        animator= GetComponent<Animator>();
        target= PlayerManager.instance.player.transform;
        agent= GetComponent<NavMeshAgent>();    
    }

    void Update()
    {
        m_Velocity = (transform.position - m_PrevPosition) / (Time.time - m_PrevTime);
        m_PrevPosition = transform.position;
        m_PrevTime = Time.time;
        if (target != null && !isAttacking)
        {
            float distance = Vector3.Distance(target.position, transform.position);

            if (distance <= lookRadius)
            {
                agent.SetDestination(target.position);

                if (distance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }

                // Set isMoving to true if the agent is currently moving
                if (agent.velocity.magnitude > 0f)
                {
                    SetIsMoving(true);
                }
                else
                {
                    SetIsMoving(false);
                }
            }
        }
        if (m_Velocity.magnitude == 0)
        {
            animator.SetBool("IsMoving", false);
        }
    }

    void FaceTarget ()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation,lookRotation,Time.deltaTime*5f);
    }

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,lookRadius);
    }

    // Sets the isMoving variable
    void SetIsMoving(bool value) {
        isMoving = value;
        animator.SetBool("IsMoving",isMoving);
    }

    public void StartAttack()
    {
        isAttacking = true;
        agent.isStopped = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
        agent.isStopped = false;
    }
}
