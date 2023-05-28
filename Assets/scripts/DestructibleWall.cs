using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    public GameObject m_BrokenWall;
    public Transform m_BrokenWallPos;

    private AudioSource m_BreakingSound;
    private MeshRenderer m_MeshRenderer;
    private BoxCollider m_BoxCollider;
    private int health = 100;

    private void Start()
    {
        m_BreakingSound = GetComponent<AudioSource>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_BoxCollider = GetComponent<BoxCollider>();
    }

    public void TakeDamage(int damage, int velocity)
    {
        if (velocity >= 5)
        {
            health -= damage;
            if (health <= 0)
            {
                Debug.Log("WALL OUT");
                m_MeshRenderer.enabled = false;
                m_BoxCollider.enabled = false;
                Instantiate(m_BrokenWall, m_BrokenWallPos.position, m_BrokenWallPos.rotation);
                m_BreakingSound.Play();
                Destroy(gameObject, 8);
            }
        }
    }
}
