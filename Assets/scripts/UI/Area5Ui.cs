using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area5Ui : MonoBehaviour
{
    public GameObject m_FinalTaskMessage;
    public GameObject m_ShoppingBag;
    public GameObject m_FlatShoppingBag;
    public GameObject m_FireTorches;
    public GameObject m_Enemies;
    public GameObject[] m_SpawnPoints;
    public GameObject m_Player;

    public void ActivateArea()
    {
        m_FinalTaskMessage.SetActive(true);
        m_ShoppingBag.SetActive(true);
        m_FlatShoppingBag.SetActive(true);
        m_FireTorches.SetActive(true);
        m_Enemies.SetActive(true);
    }

    public void SpawnPlayer()
    {
        foreach (GameObject point in m_SpawnPoints)
        {
            if (point.activeSelf)
            {
                m_Player.transform.position = point.transform.position;
                break;
            }
        }
    }


}
