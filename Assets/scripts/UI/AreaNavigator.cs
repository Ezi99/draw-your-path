using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaNavigator : MonoBehaviour
{
    public GameObject m_Area1;
    public GameObject m_Area2;
    public GameObject m_Area3;
    public GameObject m_Area4;
    public GameObject m_Area5;
    private int m_AreaIndex = 1;

    public void ActivateArea()
    {
        if (m_AreaIndex == 1)
        {
            m_Area1.GetComponent<Area1Ui>().ActivateArea();
        }
        else if (m_AreaIndex == 2)
        {
            m_Area1.SetActive(false);
            m_Area2.GetComponent<Area2Ui>().ActivateArea();
        }
        else if (m_AreaIndex == 3)
        {
            m_Area2.SetActive(false);
            m_Area3.GetComponent<Area3Ui>().ActivateArea();
        }
        else if (m_AreaIndex == 4)
        {
            m_Area3.SetActive(false);
            m_Area4.GetComponent<Area4Ui>().ActivateArea();
        }
        else if (m_AreaIndex == 5)
        {
            m_Area4.SetActive(false);
            m_Area5.GetComponent<Area5Ui>().ActivateArea();

        }

        m_AreaIndex++;
    }

    public void ReSpawnPlayer()
    {
        if (m_AreaIndex == 1)
        {
            m_Area1.GetComponent<Area1Ui>().SpawnPlayer();
        }
        if (m_AreaIndex == 2)
        {
            m_Area2.GetComponent<Area2Ui>().SpawnPlayer();
        }
        if (m_AreaIndex == 3)
        {
            m_Area3.GetComponent<Area3Ui>().SpawnPlayer();
        }
        if (m_AreaIndex == 4)
        {
            m_Area4.GetComponent<Area4Ui>().SpawnPlayer();
        }
        else
        {
            m_Area5.GetComponent<Area5Ui>().SpawnPlayer();
        }
    }
}
