using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private AudioSource m_ArrowLaunchSound;

    private void Start()
    {
        m_ArrowLaunchSound = GetComponent<AudioSource>();
        m_ArrowLaunchSound.Play();
        Destroy(gameObject, 10);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Arrow Hit");
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(10);
            Destroy(gameObject); // Destroy the arrow game object
        }
        else if (collision.gameObject.CompareTag("Shield"))
        {
            collision.gameObject.GetComponent<Shield>().TakeDamage(10);
        }
    }
}
