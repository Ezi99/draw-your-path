using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Area4Ui : MonoBehaviour
{
    public GameObject m_BreakingWallHint;
    public GameObject m_FireTorches;
    public GameObject[] m_SpawnPoints;
    public GameObject m_Player;


    public void ActivateArea()
    {
        m_FireTorches.SetActive(true);
    }

    public void ShowBreakingWallHint()
    {
        m_BreakingWallHint.SetActive(true);
        Invoke("hideBreakingWallHint", 30);
    }

    private void hideBreakingWallHint()
    {
        m_BreakingWallHint.GetComponent<WallTutorial>().CollapseWall();
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
