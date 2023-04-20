using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private int m_Damage;

    private void Start()
    {
        Destroy(gameObject, 5);
    }

    public void SetStats(int damage)
    {
        m_Damage = damage;
    }
}
