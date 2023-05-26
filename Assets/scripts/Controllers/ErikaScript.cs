using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErikaScript : MonoBehaviour
{
    private Animator animator;
    public GameObject arrowObj;
    public Transform arrowPoint;
    public Transform playerTransform;
    public int health=100;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void shoot()
    {
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
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("Death animation triggered");
        animator.SetBool("Death", true);
    }
    public void destroy()
    {
        Destroy(gameObject);
    }
}
