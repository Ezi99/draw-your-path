using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area3Ui : MonoBehaviour
{
    public GameObject m_AreaGoal;
    public GameObject m_Enemies;

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        m_AreaGoal.SetActive(true);
    }

    public void SpawnEnemies()
    {
        m_Enemies.SetActive(true);
    }
}