using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area3Ui : MonoBehaviour
{
    public GameObject m_AreaGoalMessage;
    public GameObject m_Enemies;
    public AreaNavigator m_AreaNavigator;
    public GameObject m_PuzzleFinishedMessage;

    void Start()
    {
        
    }

    public void ActivateArea()
    {
        m_AreaGoalMessage.SetActive(true);
    }

    public void SpawnEnemies()
    {
        m_Enemies.SetActive(true);
    }

    public void PuzzleFinished()
    {
        Invoke("showPuzzleFinishedMessage", 3);
    }

    private void showPuzzleFinishedMessage()
    {
        m_PuzzleFinishedMessage.SetActive(true);
        Destroy(m_PuzzleFinishedMessage, 10);
    }

}