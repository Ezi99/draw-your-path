using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Sword : MonoBehaviour
{
    public float VelocityLimitToDamage;
    public float m_DamageCooldown; // Adjust the cooldown duration as needed

    private AudioSource m_SwordHitSound;
    private int m_Damage = 100;
    private int m_Durability = 1000;
    private bool m_CanDamage = true;
    private Vector3 m_PrevPosition;
    private Vector3 m_Velocity;
    private float m_PrevTime;


    private void Start()
    {
        m_PrevPosition = transform.position;
        m_PrevTime = Time.time;
        m_SwordHitSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        m_Velocity = (transform.position - m_PrevPosition) / (Time.time - m_PrevTime);
        m_PrevPosition = transform.position;
        m_PrevTime = Time.time;
    }

    public void SetStats(int i_Damage, int i_Durability)
    {
        m_Damage = i_Damage;
        m_Durability = i_Durability;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("velocity.magnitude " + m_Velocity.magnitude);
        if (m_Velocity.magnitude > VelocityLimitToDamage)
        {
            if (m_CanDamage && other.tag.Contains("Erika"))
            {
                Debug.Log("Erika");
                ErikaScript erika = other.GetComponentInParent<ErikaScript>();
                m_SwordHitSound.Play();
                if (erika != null)
                {
                    Debug.Log("stabbed");
                    m_Durability -= 10;
                    erika.takeDamage(m_Damage);
                    m_CanDamage = false;
                    Invoke("ResetDamageCooldown", m_DamageCooldown);

                }
            }
            else if (m_CanDamage && other.tag.Contains("Paladin"))
            {
                Debug.Log("Paladin");
                PaladinScript paladin = other.GetComponentInParent<PaladinScript>();
                m_SwordHitSound.Play();
                if (paladin != null)
                {
                    Debug.Log("stabbed");
                    m_Durability -= 10;
                    if (other.tag.Contains("Head"))
                    {
                        paladin.takeDamage(m_Damage, true);
                    }
                    else
                        paladin.takeDamage(m_Damage, false);
                    m_CanDamage = false;
                    Invoke("ResetDamageCooldown", m_DamageCooldown);
                }
            }
            else if (m_CanDamage && other.CompareTag("Paladin_Shield"))
            {
                Debug.Log("Blocked");
                m_SwordHitSound.Play();
                m_Durability -= 10;
                m_CanDamage = false;
                Invoke("ResetDamageCooldown", m_DamageCooldown);
            }

            if (m_Durability < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ResetDamageCooldown()
    {
        m_CanDamage = true;
    }
}