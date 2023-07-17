using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area1Ui : MonoBehaviour
{
    public GameObject m_StaticEnemyMessage;
    public GameObject m_BlockingEnemyMessage;
    public GameObject m_MovingEnemyMessage;
    public GameObject m_FireBallMessage;
    public GameObject m_HintsMessage;
    public GameObject[] m_SpawnPoints;
    public GameObject m_Player;
    private int m_MessageCounter = 0;

    public void ActivateArea()
    {
        m_StaticEnemyMessage.SetActive(true);
        Destroy(m_StaticEnemyMessage, 10);
    }

    public void ShowMessage()
    {
        if (m_MessageCounter == 0)
        {
            showBlockingEnemyMessage();
        }
        else if (m_MessageCounter == 1)
        {
            showMovingEnemyMessage();
        }
        else
        {
            showHintMessage();
        }

        m_MessageCounter++;
    }
    private void showBlockingEnemyMessage()
    {
        m_BlockingEnemyMessage.SetActive(true);
    }

    private void showMovingEnemyMessage()
    {
        m_BlockingEnemyMessage.SetActive(false);
        m_MovingEnemyMessage.SetActive(true);
        m_FireBallMessage.SetActive(true);
    }

    private void showHintMessage()
    {
        m_MovingEnemyMessage.SetActive(false);
        m_FireBallMessage.SetActive(false);
        m_HintsMessage.SetActive(true);
        Invoke("hideHintMessage", 15);
    }

    private void hideHintMessage()
    {
        m_HintsMessage.SetActive(false);
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
