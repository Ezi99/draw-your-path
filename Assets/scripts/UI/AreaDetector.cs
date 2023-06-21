using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDetector : MonoBehaviour
{
    public AreaNavigator m_AreaNavigator;
    private bool m_DidCollide = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && m_DidCollide == false)
        {
            m_AreaNavigator.ActivateArea();
            m_DidCollide = true;
        }
    }
}
