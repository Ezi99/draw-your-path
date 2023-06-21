using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GreenPotion : MonoBehaviour
{
    protected static int s_NumOfPotionsTaken;
    public VillageGate leftGate;
    public VillageGate rightGate;
    public AudioManager m_AudioManager;
    public GameObject[] m_PotionTakenMessages;
    public Area3Ui m_Area3Ui;

    public void WhenSelected()
    {
        m_AudioManager.PlayPotionTakenSound();
        leftGate.PotionTaken();
        rightGate.PotionTaken();
        BeforeDespawn();
        Destroy(gameObject);
    }

    protected virtual void BeforeDespawn()
    {
        for (int i = 0; i < m_PotionTakenMessages.Length; i++)
        {
            if (i == s_NumOfPotionsTaken)
            {
                m_PotionTakenMessages[i].SetActive(true);
            }
            else
            {
                m_PotionTakenMessages[i].SetActive(false);
            }
        }

        s_NumOfPotionsTaken++;
        if(s_NumOfPotionsTaken == m_PotionTakenMessages.Length)
        {
            m_Area3Ui.PuzzleFinished();
        }
    }
}
