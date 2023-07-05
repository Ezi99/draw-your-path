using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWallDetector : MonoBehaviour
{
    public WallTutorial m_WallTutorial;
    public Area1Ui m_AreaNavigator;
    private bool m_Triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && m_Triggered == false)
        {
            m_WallTutorial.CollapseWall();
            m_AreaNavigator.ShowMessage();
            m_Triggered = true;
        }
    }
}
