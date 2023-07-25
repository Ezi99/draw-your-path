using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bridge : MonoBehaviour
{
    public Text m_Timer1;
    public Text m_Timer2;
    public int m_TimeLeft = -1;
    public bool m_CountDown;

    private void Update()
    {
        if (m_TimeLeft >= 0 && m_CountDown == true)
        {
            UpdateTimer(m_TimeLeft);
            m_CountDown = false;
        }

    }

    public void DespawnCountDown(int lifeSpan)
    {
        Destroy(gameObject, lifeSpan);
        m_TimeLeft = lifeSpan;
        m_CountDown = true;
    }

    public void WhenSelected()
    {
        gameObject.layer = LayerMask.NameToLayer("grabbed bridge");
    }

    public void WhenExited()
    {
        gameObject.layer = LayerMask.NameToLayer("bridge");
    }

    private void decreaseTime()
    {
        m_TimeLeft--;
        m_CountDown = true;
    }

    private void UpdateTimer(int currentTime)
    {
        m_Timer1.text = currentTime.ToString();
        m_Timer2.text = currentTime.ToString();
        Invoke("decreaseTime", 1);
    }
}
