using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area1Ui : MonoBehaviour
{
    public GameObject m_StaticEnemyMessage;
    public GameObject m_BlockingEnemyMessage;
    public GameObject m_MovingEnemyMessage;
    public GameObject m_HintsMessage;
    public AreaUiNavigator m_UiNavigator;
    private bool m_DidCollide = false;
    private int m_MessageCounter = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (m_DidCollide == false)
            {
                m_UiNavigator.ActivateArea1();
                m_StaticEnemyMessage.SetActive(true);
                Destroy(m_StaticEnemyMessage, 10);
                m_DidCollide = true;
            }
        }
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
    }

    private void showHintMessage()
    {
        m_MovingEnemyMessage.SetActive(false);
        m_HintsMessage.SetActive(true);
        Invoke("hideHintMessage", 15);
    }

    private void hideHintMessage()
    {
        m_HintsMessage.SetActive(false);
    }
}
