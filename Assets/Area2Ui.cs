using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Area2Ui : MonoBehaviour
{
    public Transform m_PlayersHead;
    public GameObject m_PuzzleMessage;
    public GameObject m_FirstEnemyWave;
    public GameObject m_SecondEnemyWave;
    public AreaUiNavigator m_AreaUiNavigator;
    private bool m_IsDone = false;

    private void OnTriggerEnter(Collider other)
    {
        m_AreaUiNavigator.ActivateArea2();
        m_FirstEnemyWave.SetActive(true);
        m_SecondEnemyWave.SetActive(true);
    }
    void Update()
    {
        if (m_FirstEnemyWave.transform.childCount == 0 && m_IsDone == false)
        {
            Invoke("showHintMessage", 10);
            m_IsDone = true;
        }
        m_PuzzleMessage.transform.LookAt(new Vector3(m_PlayersHead.position.x, m_PuzzleMessage.transform.position.y, m_PlayersHead.position.z));
        m_PuzzleMessage.transform.forward *= -1;
    }
    private void showHintMessage()
    {
        m_PuzzleMessage.SetActive(true);
        Invoke("hideHintMessage", 120);
    }

    private void hideHintMessage()
    {
        m_PuzzleMessage.SetActive(false);
    }
}
