using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordAttack : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag=="Player")
        {
            Debug.Log("ayyyy");
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(10);
        }
    }
}
