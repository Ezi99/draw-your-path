using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageGate : MonoBehaviour
{
    public int potionCounter;

    private Rigidbody rigidBody;
    private AudioSource m_AudioSource;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int velocity)
    {
        if(potionCounter <= 0 && velocity > 4)
        {
            rigidBody.isKinematic = false;
            rigidBody.useGravity = true;
            m_AudioSource.Play();
            rigidBody.AddForce(100 * velocity * transform.forward);
        }
    }

    public void PotionTaken()
    {
        potionCounter--;
    }
}
