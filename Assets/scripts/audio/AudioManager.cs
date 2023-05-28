using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip m_BreakingWallSound;

    private AudioSource m_AudioSource;
    

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlayWallBreakingSound()
    {
        m_AudioSource.clip = m_BreakingWallSound;
        m_AudioSource.Play();
    }
}
