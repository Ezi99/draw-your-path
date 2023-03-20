using System;
using UnityEngine;
using UnityEngine.Events;

namespace OctoXR.Samples.KinematicInteractions
{
    public class Target : MonoBehaviour
    {
        [SerializeField] private float health = 10f;
        public UnityEvent OnHit;

        public void TakeDamage(float amount)
        {
            health -= amount;
            if(health <= 0)
            {
                OnHit.Invoke();
            }
        }
    }
}