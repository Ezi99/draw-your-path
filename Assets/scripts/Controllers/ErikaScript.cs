using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ErikaScript : MonoBehaviour
{
    
    public List<Rigidbody> rigidbodies;
    private Animator m_Animator;
    public GameObject arrowObj;
    public Transform arrowPoint;
    public Transform playerTransform;
    public int health=100;
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
       
    }
    public void shoot()
    {
        Debug.Log("Arrow shot");
        GameObject arrow = Instantiate(arrowObj, arrowPoint.position, Quaternion.identity);
        Vector3 direction = playerTransform.position+new Vector3(0,1.7f,0) - arrowPoint.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        arrow.transform.rotation = rotation;
        arrow.GetComponent<Rigidbody>().AddForce(arrow.transform.forward * 25, ForceMode.Impulse);
    }
    public void takeDamage(int dmg)
    {
        
        health -= dmg;
        Debug.Log($"Erika Health -{dmg} current health {health}");
        if (health<=0)
        {
                m_Animator.enabled = false;
            //Die();
            foreach(var item in rigidbodies)
            {
                item.isKinematic = false;
                item.useGravity = true;
            }
        }
    }
    private void Die()
    {
        Debug.Log("Death animation triggered");
        m_Animator.SetBool("Death", true);
    }
    public void destroy()
    {
        Destroy(gameObject);
    }
}
