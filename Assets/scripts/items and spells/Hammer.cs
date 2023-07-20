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
    private readonly int m_HitDamageToDurability = 10;
    private bool canDamage = true;
    private Vector3 m_PrevPosition;
    private Vector3 m_Velocity;
    private float m_PrevTime;
    private AudioSource m_HammerHitSound;
    public float deleteText = 5;

    private void Start()
    {
        m_HammerHitSound = GetComponent<AudioSource>();
        m_PrevPosition = transform.position;
        m_PrevTime = Time.time;
        Durability.maxValue = m_Durability;
        Invoke("DeleteText", deleteText);
    }

    private void Update()
    {
        m_Velocity = (transform.position - m_PrevPosition) / (Time.time - m_PrevTime);
        m_PrevPosition = transform.position;
        m_PrevTime = Time.time;
        Durability.value = m_Durability;
    }

    public void SetStats(int dmg, int Durability)
    {
        m_Damage = dmg;
        m_Durability = Durability;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Velocity.magnitude > m_VelocityLimitToDamage)
        {
            if (canDamage && other.CompareTag("Erika"))
            {
                Debug.Log("Erika");
                ErikaScript erika = other.GetComponentInParent<ErikaScript>();

                m_HammerHitSound.Play();
                if (erika != null)
                {
                    Debug.Log("hammered");
                    m_Durability -= m_HitDamageToDurability;
                    erika.takeDamage(m_Damage);
                    canDamage = false;
                    Invoke("ResetDamageCooldown", m_DamageCooldown);
                }
            }
            else if (canDamage && other.tag.Contains("Paladin"))
            {
                Debug.Log("Paladin");
                PaladinScript paladin = other.GetComponentInParent<PaladinScript>();

                m_HammerHitSound.Play();
                if (paladin != null)
                {

                    Debug.Log("hammered");
                    m_Durability -= m_HitDamageToDurability;
                    if (other.tag.Contains("Head"))
                    {
                        paladin.takeDamage(m_Damage, true);
                    }
                    else
                        paladin.takeDamage(m_Damage, false);
                    canDamage = false;
                    Invoke("ResetDamageCooldown", m_DamageCooldown);
                }
            }
            else if (canDamage && other.CompareTag("Paladin_Shield"))
            {
                Debug.Log("Blocked");
                m_HammerHitSound.Play();
                m_Durability -= m_HitDamageToDurability;
                canDamage = false;
                Invoke("ResetDamageCooldown", m_DamageCooldown);
            }
            else if (other.CompareTag("destructible wall") == true)
            {
                DestructibleWall wall = other.GetComponent<DestructibleWall>();

                m_HammerHitSound.Play();
                if (wall != null)
                {
                    wall.TakeDamage(m_Damage, (int)m_Velocity.magnitude);
                }
            }
            else if (other.CompareTag("Gate") == true)
            {
                VillageGate gate = other.GetComponent<VillageGate>();

                m_HammerHitSound.Play();
                if (gate != null)
                {
                    gate.TakeDamage((int)m_Velocity.magnitude);
                }
            }

            if (m_Durability < 0)
            {
                Destroy(gameObject);
            }
        }

    }

    private void ResetDamageCooldown()
    {
        canDamage = true;
    }
    private void DeleteText()
    {
        Transform kak = transform.Find("Canvas");
        kak.gameObject.SetActive(false);
    }
}
