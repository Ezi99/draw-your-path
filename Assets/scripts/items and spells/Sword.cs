using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private int m_Damage;
    private int m_Durability;

    void Update()
    {

    }

    public void SetStats(int damage, int Durability)
    {
        m_Damage = damage;
        m_Durability = Durability;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") == true)
        {
            //other.getObje
        }
    }
}