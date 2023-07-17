using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    private int m_Durability=100;
    public Slider durability;
    public float deleteText = 5;
    private void Start()
    {
        durability.maxValue = m_Durability;
        Invoke("DeleteText", deleteText);
    }
    void Update()
    {
        durability.value= m_Durability;
        if (m_Durability==0)
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
        m_Durability -= dmg;
    }
    private void DeleteText()
    {
        Transform kak = transform.Find("Canvas");
        kak.gameObject.SetActive(false);
    }
}
