using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sword : MonoBehaviour
{
    public float VelocityLimitToDamage;
    public float m_DamageCooldown; // Adjust the cooldown duration as needed
    public Slider Durability;
    private AudioSource m_AudioSource;
    public AudioClip m_ShieldHitSound;
    public AudioClip m_SwordHitSound;
    private int m_Damage = 34;
    public int m_Durability = 100;
    private const int m_DamageToDurability = 10;
    private bool m_CanDamage = true;
    private Vector3 m_PrevPosition;
    private Vector3 m_Velocity;
    private float m_PrevTime;
    public float deleteText = 5;
    private bool m_WasGrabbed = false;

    private void Start()
    {
        m_PrevPosition = transform.position;
        m_PrevTime = Time.time;
        m_AudioSource = GetComponent<AudioSource>();
        Durability.maxValue = m_Durability;
        Invoke("DeleteText", deleteText);
    }

    private void Update()
    {
        m_Velocity = (transform.position - m_PrevPosition) / (Time.time - m_PrevTime);
        m_PrevPosition = transform.position;
        m_PrevTime = Time.time;
        Durability.value = m_Durability;
        if (m_Durability <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetStats(int i_Damage, int i_Durability)
    {
        m_Damage = i_Damage;
        m_Durability = i_Durability;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Velocity.magnitude > VelocityLimitToDamage)
        {
            m_AudioSource.clip = m_SwordHitSound;
            if (m_CanDamage && other.tag.Contains("Erika"))
            {
                ErikaScript erika = other.GetComponentInParent<ErikaScript>();

                if (erika != null)
                {
                    m_AudioSource.Play();
                    m_Durability -= m_DamageToDurability;
                    Durability.value = m_Durability;
                    erika.takeDamage(m_Damage);
                    m_CanDamage = false;
                    Invoke("ResetDamageCooldown", m_DamageCooldown);
                }
            }
            else if (m_CanDamage && other.tag.Contains("Paladin"))
            {
                PaladinScript paladin = other.GetComponentInParent<PaladinScript>();

                if (paladin != null)
                {
                    m_Durability -= m_DamageToDurability;
                    if (other.tag.Contains("Head"))
                    {
                        paladin.takeDamage(m_Damage * 2, true);
                    }
                    else if (other.CompareTag("Paladin_Shield"))
                    {
                        Debug.Log("Paladin Blocked");
                        m_AudioSource.clip = m_ShieldHitSound;
                    }
                    else
                    {
                        paladin.takeDamage(m_Damage, false);
                    }

                    m_AudioSource.Play();
                    m_CanDamage = false;
                    Invoke("ResetDamageCooldown", m_DamageCooldown);
                }
            }
        }
    }

    private void ResetDamageCooldown()
    {
        m_CanDamage = true;
    }

    private void DeleteText()
    {
        Transform kak = transform.Find("Canvas");
        kak.gameObject.SetActive(false);
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

}