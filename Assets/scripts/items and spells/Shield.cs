using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private int m_Durability;
    public DestructibleWall Wall;

    void Update()
    {

    }

    public void SetStats(int Durability)
    {
        m_Durability = Durability;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("destructible wall") == true)
        {
            Wall.DestoryWall();
        }
    }

}
