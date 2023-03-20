using UnityEngine;

namespace OctoXR.Samples.KinematicInteractions
{
    public class FingerInteractor : MonoBehaviour
    {
        [SerializeField] private Transform fingerTipTransform;
        [SerializeField] private float colliderRadius = 0.01f;
        private SphereCollider sphereCollider;
        
        void Start()
        {
            AdjustPosition();

            InstantiateCollider();
        }

        private void AdjustPosition()
        {
            Transform thisTransform;
            (thisTransform = transform).SetParent(fingerTipTransform);
            thisTransform.localPosition = Vector3.zero;
            thisTransform.localRotation = Quaternion.identity;
        }

        private void InstantiateCollider()
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = colliderRadius;
            sphereCollider.isTrigger = true;
        }
    }
}
