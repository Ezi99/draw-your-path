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
    public GameObject[] m_SpawnPoints;
    public GameObject m_Player;

    private bool m_IsDone = false;

    public void ActivateArea()
    {
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
