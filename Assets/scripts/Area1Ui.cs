using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area1Ui : MonoBehaviour
{
    public GameObject m_StaticEnemyMessage;
    public GameObject m_BlockingEnemyMessage;
    public GameObject m_MovingEnemyMessage;
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
            ShowBlockingEnemyMessage();
            m_MessageCounter++;
        }
        else
        {
            ShowMovingEnemyMessage();
        }
    }
    private void ShowBlockingEnemyMessage()
    {
        m_BlockingEnemyMessage.SetActive(true);
    }

    private void ShowMovingEnemyMessage()
    {
        m_BlockingEnemyMessage.SetActive(false);
        m_MovingEnemyMessage.SetActive(true);
    }

}
