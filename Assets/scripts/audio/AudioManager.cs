using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource m_DesertAudio;
    public AudioClip m_PotionTakenAudioClip;

    private AudioSource m_AudioSource;
    

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();    
    }

    public void PauseDesertAudio()
    {
        m_DesertAudio.Pause();
    }

    public void PlayPotionTakenSound()
    {
        m_AudioSource.clip = m_PotionTakenAudioClip;
        m_AudioSource.Play();
    }
}
