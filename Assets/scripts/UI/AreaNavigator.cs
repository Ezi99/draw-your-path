using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaNavigator : MonoBehaviour
{
    public GameObject m_Area1;
    public GameObject m_Area2;
    public GameObject m_Area3;
    int m_AreaIndex = 1;

    void Start()
    {

    }

    void Update()
    {

    }

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

        m_AreaIndex++;
    }
}
