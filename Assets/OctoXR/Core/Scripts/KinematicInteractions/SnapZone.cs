using System;
using UnityEngine;
using UnityEngine.Events;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Object that looks for grabbable components and snaps them inside after an OnTriggerEnter event.
    /// This object has to have a trigger collider on it to work.
    /// </summary>

    public class SnapZone : MonoBehaviour, IAttachable<Transform>
    {
        [SerializeField] private Transform objectTransform;
        [SerializeField] private float radius;
        [SerializeField] private LayerMask layer;
        [SerializeField] private Grabbable currentGrabbable;

        [SerializeField] private UnityEvent OnSnapped;
        [SerializeField] private UnityEvent OnUnsnapped;

        [SerializeField] private bool hasObject;
        [SerializeField] private MeshRenderer meshRenderer;

        private Transform snapZoneTransform;

        private Grabbable previousGrabbable;

        private void Start()
        {
            objectTransform = transform;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!gameObject.activeSelf) return;

            currentGrabbable = other.GetComponent<Grabbable>();

            if (!currentGrabbable) return;

            if (!currentGrabbable.IsGrabbed)
            {
                //Attach(transform);
                //currentGrabbable.OnAttach.AddListener(Detach);
            }

            //currentGrabbable.OnDetach.AddListener(delegate { Attach(transform); });
        }

        private void OnTriggerStay(Collider other)
        {
            if (currentGrabbable && !currentGrabbable.IsGrabbed)
            {
                Attach(transform);
                //currentGrabbable.OnAttach.AddListener(Detach);
            }
            else
            {
                Detach();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!gameObject.activeSelf) return;

            // if (previousGrabbable && previousGrabbable != currentGrabbable)
            // {
            //     previousGrabbable.OnDetach.RemoveListener(delegate { Attach(transform); });
            //     previousGrabbable.OnAttach.RemoveListener(Detach);
            // }

            if (!hasObject) return;
            previousGrabbable = currentGrabbable;

            currentGrabbable = null;
            //OnUnsnapped.Invoke();

            //hasObject = false;
        }

        public void Attach(Transform transform)
        {
            if (hasObject) return;

            Collider[] hitColliders = UnityEngine.Physics.OverlapSphere(transform.position, radius, layer);

            if (hitColliders.Length > 0)
            {
                var foundObject = hitColliders[0].transform;

                foundObject.GetComponent<Rigidbody>().isKinematic = true;
                foundObject.transform.position = transform.position;
                foundObject.transform.rotation = transform.rotation;
                meshRenderer.enabled = false;

                foundObject.transform.SetParent(transform);

                OnSnapped.Invoke();
            }

            hasObject = true;
            Debug.Log("#Attached");
        }

        public void Detach()
        {
            if (!hasObject) return;

            meshRenderer.enabled = true;
            OnUnsnapped.Invoke();
            hasObject = false;

            Debug.Log("#Detached");

            //currentGrabbable.OnAttach.RemoveListener(Detach);
        }
    }
}
