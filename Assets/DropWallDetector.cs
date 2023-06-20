using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWallDetector : MonoBehaviour
{
    public WallTutorial m_WallTutorial;
    public Area1Ui m_AreaNavigator;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            m_WallTutorial.CollapseWall();
            m_AreaNavigator.ShowMessage();
        }
    }
}
