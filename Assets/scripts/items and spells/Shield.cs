using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private int m_Durability;

    void Update()
    {

    }

    public void SetStats(int Durability)
    {
        m_Durability = Durability;
    }
}
