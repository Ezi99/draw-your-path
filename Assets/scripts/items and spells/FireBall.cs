using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public GameObject m_ExplosionVisualEffect;
    public GameObject m_TrailingFireVisualEffect;
    public AudioClip m_ExplosionSound;

    private int m_Damage = 100;
    private AudioSource m_AudioSource;
    private MeshRenderer m_FireBallRenderer;
    private Rigidbody m_FireBallRigidbody;
    private bool m_Exploded = false;
   

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_FireBallRenderer = GetComponent<MeshRenderer>();
        m_FireBallRigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Exploded == false)
        {
            m_TrailingFireVisualEffect.SetActive(false);
            m_FireBallRigidbody.useGravity = false;
            m_FireBallRigidbody.isKinematic = true;
            m_AudioSource.clip = m_ExplosionSound;
            m_ExplosionVisualEffect.SetActive(true);
            m_FireBallRenderer.enabled = false;
            m_AudioSource.Play();

            if (other.CompareTag("Erika"))
            {
                Debug.Log("Erika");
                ErikaScript erika = other.GetComponentInParent<ErikaScript>();

                if (erika != null)
                {
                    Debug.Log("NUKED");
                    erika.takeDamage(m_Damage);
                }
            }
            else if (other.CompareTag("Paladin"))
            {
                Debug.Log("Paladin");
                PaladinScript paladin = other.GetComponentInParent<PaladinScript>();

                if (paladin != null)
                {
                    Debug.Log("NUKED");
                    paladin.takeDamage(m_Damage);
                }
            }

            m_Exploded = true;
            Destroy(gameObject, 1);
        }
    }

    public void SetStats(int dmg)
    {
        m_Damage = dmg;
    }
}