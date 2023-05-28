using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTombAudio : MonoBehaviour
{
    public List<AudioSource> m_torches;

    private AudioSource m_CaveSound;
    private bool m_IsPlaying;

    private void Start()
    {
        m_CaveSound = GetComponent<AudioSource>();
        m_IsPlaying = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (m_IsPlaying == false)
            {
                m_CaveSound.Play();
                foreach (AudioSource torch in m_torches)
                {
                    torch.Play();
                }
                m_IsPlaying = true;
            }
        }
    }
}
