using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hammer : MonoBehaviour
{
    private int m_Damage;
    private int m_Durability;
    private Vector3 prevPosition;
    private Vector3 velocity;
    private float prevTime;


    private void Start()
    {
        prevPosition = transform.position;
        prevTime = Time.time;
    }

    private void Update()
    {
        velocity = (transform.position - prevPosition) / (Time.time - prevTime);
        prevPosition = transform.position;
        prevTime = Time.time;
    }

    public void SetStats(int damage, int Durability)
    { 
        m_Damage = damage;
        m_Durability = Durability;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("destructible wall") == true)
        {
            DestructibleWall wall = other.GetComponent<DestructibleWall>();
            if (wall != null)
            {
                wall.TakeDamage(m_Damage, (int)velocity.magnitude);
            }
        }
    }
}
