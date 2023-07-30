using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class Hammer : MonoBehaviour
{
    public float m_VelocityLimitToDamage;
    public float m_DamageCooldown;
    public Slider Durability;
    private int m_Damage = 33;
    private int m_Durability = 100;
    private const int m_DamageToDurability = 10;
    private bool m_CanDamage = true;
    private Vector3 m_PrevPosition;
    private Vector3 m_Velocity;
    private float m_PrevTime;
    private AudioSource m_AudioSource;
    public AudioClip m_ShieldHitSound;
    public AudioClip m_HammerHitSound;
    public float deleteTextTimer = 5;
    private bool m_WasGrabbed = false;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_PrevPosition = transform.position;
        m_PrevTime = Time.time;
        Durability.maxValue = m_Durability;
        Invoke("DeleteText", deleteTextTimer);
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

    public void SetStats(int dmg, int Durability)
    {
        m_Damage = dmg;
        m_Durability = Durability;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Velocity.magnitude > m_VelocityLimitToDamage && m_CanDamage)
        {
            m_AudioSource.clip = m_HammerHitSound;
            if (other.tag.Contains("Erika"))
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
            else if (other.tag.Contains("Paladin"))
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
            else if (other.CompareTag("destructible wall") == true)
            {
                DestructibleWall wall = other.GetComponent<DestructibleWall>();

                if (wall != null)
                {
                    m_AudioSource.Play();
                    wall.TakeDamage(m_Damage, (int)m_Velocity.magnitude);
                }
            }
            else if (other.CompareTag("Gate") == true)
            {
                VillageGate gate = other.GetComponent<VillageGate>();

                if (gate != null)
                {
                    m_AudioSource.Play();
                    gate.TakeDamage((int)m_Velocity.magnitude);
                }
            }
        }
    }

    private void ResetDamageCooldown()
    {
        m_CanDamage = true;
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
