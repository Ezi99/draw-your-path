using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    private int m_Durability=100;
    public Slider durability;
    public float deleteTextTimer = 5;
    private AudioSource m_ShieldBlockSound;
    private bool m_WasGrabbed = false;

    private void Start()
    {
        m_ShieldBlockSound = GetComponent<AudioSource>();
        durability.maxValue = m_Durability;
        Invoke("DeleteText", deleteTextTimer);
    }

    void Update()
    {
        durability.value= m_Durability;
        if (m_Durability <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetStats(int Durability)
    {
        m_Durability = Durability;
    }

    public void TakeDamage(int dmg)
    {
        m_ShieldBlockSound.Play();
        m_Durability -= dmg;
    }

    public void WhenSelected()
    {
        if (m_WasGrabbed == false)
        {
            gameObject.layer = LayerMask.NameToLayer("grabbable");
            m_WasGrabbed = true;
            DeleteText();
        }
    }

    private void DeleteText()
    {
        Transform text = transform.Find("Canvas");
        text.gameObject.SetActive(false);
    }
}
