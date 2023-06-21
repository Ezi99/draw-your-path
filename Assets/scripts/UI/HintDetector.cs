using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintDetector : MonoBehaviour
{
    private bool m_Triggered = false;
    public GameObject m_FirstWaveEnemies;
    public Area4Ui m_Area4Ui;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && m_FirstWaveEnemies.transform.childCount == 0 && m_Triggered == false)
        {
            m_Area4Ui.ShowBreakingWallHint();
            m_Triggered = true;
        }
    }
}
