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
    public int m_NextAreaToActivate;

    public void ActivateArea()
    {
        if (m_NextAreaToActivate == 1)
        {
            m_Area1.GetComponent<Area1Ui>().ActivateArea();
        }
        else if (m_NextAreaToActivate == 2)
        {
            m_Area1.SetActive(false);
            m_Area2.GetComponent<Area2Ui>().ActivateArea();
        }
        else if (m_NextAreaToActivate == 3)
        {
            m_Area2.SetActive(false);
            m_Area3.GetComponent<Area3Ui>().ActivateArea();
        }
        else if (m_NextAreaToActivate == 4)
        {
            m_Area3.SetActive(false);
            m_Area4.GetComponent<Area4Ui>().ActivateArea();
        }
        else if (m_NextAreaToActivate == 5)
        {
            m_Area4.SetActive(false);
            m_Area5.GetComponent<Area5Ui>().ActivateArea();

        }

        m_NextAreaToActivate++;
    }

    public void ReSpawnPlayer()
    {
        if (m_NextAreaToActivate - 1 == 1)
        {
            m_Area1.GetComponent<Area1Ui>().SpawnPlayer();
        }
        else if (m_NextAreaToActivate - 1 == 2)
        {
            m_Area2.GetComponent<Area2Ui>().SpawnPlayer();
        }
        else if (m_NextAreaToActivate - 1 == 3)
        {
            m_Area3.GetComponent<Area3Ui>().SpawnPlayer();
        }
        else if (m_NextAreaToActivate - 1 == 4)
        {
            m_Area4.GetComponent<Area4Ui>().SpawnPlayer();
        }
        else
        {
            m_Area5.GetComponent<Area5Ui>().SpawnPlayer();
        }
    }
}
