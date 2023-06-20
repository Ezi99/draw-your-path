using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area2Ui : MonoBehaviour
{
    public GameObject m_PuzzleMessage;
    public GameObject m_Enemies;

    void Start()
    {

    }

    void Update()
    {
        if (m_Enemies.transform.childCount == 0)
        {
            Debug.Log("they dead !");
            m_PuzzleMessage.SetActive(true);
        }
    }
}
