using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Area4Ui : MonoBehaviour
{
    public GameObject m_BreakingWallHint;

    public void ActivateArea()
    {
        
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
}
