using UnityEngine;

namespace OctoXR.Samples.KinematicInteractions
{
    public class ParticleController : MonoBehaviour
    {
        [SerializeField] private Transform source;
        [SerializeField] private GameObject attackEffect;
        [SerializeField] private GameObject hitEffect;

        private readonly float particleDestroyTime = 1f;

        public void PlayAttackParticle()
        {
            var impact = Instantiate((Object) attackEffect, source.localPosition, source.localRotation);
            Destroy(impact, particleDestroyTime);
        }

        public void PlayImpactParticle(RaycastHit hit)
        {
            var impact = Instantiate((Object) hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, particleDestroyTime);
        }
    }
}