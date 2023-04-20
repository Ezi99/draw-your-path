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
    // Start is called before the first frame update
    void Start()
    {
        animator= GetComponent<Animator>();
        target= PlayerManager.instance.player.transform;
        agent= GetComponent<NavMeshAgent>();    
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position,transform.position);

        if (distance<= lookRadius)
        {
            agent.SetDestination(target.position);

            if (distance<= agent.stoppingDistance)
            {
                FaceTarget();
            }

            // Set isMoving to true if the agent is currently moving
            if (agent.velocity.magnitude > 0f) {
                SetIsMoving(true);
            }
            else {
                SetIsMoving(false);
            }
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
}