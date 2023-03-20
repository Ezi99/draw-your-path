using System;
using System.Collections;
using OctoXR.KinematicInteractions.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Main component for adding grabbable features to a game object.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(VelocityEstimator))]
    public class Grabbable : MonoBehaviour, IAttachable<Transform>
    {
        [SerializeField] private GrabbableModifier grabbableModifier;
        public GrabbableModifier GrabbableModifier { get => grabbableModifier; set => grabbableModifier = value; }

        [Tooltip("If ticked, the grabbable object won't snap to the hand's position.")]
        [SerializeField] private bool isPrecisionGrab = true;
        public bool IsPrecisionGrab { get => isPrecisionGrab; set => isPrecisionGrab = value; }

        [Tooltip("If ticked, the grabbable object will reset to It's original position on collision with the object ")]
        [SerializeField] private bool isRespawnable;
        public bool IsRespawnable { get => isRespawnable; set => isRespawnable = value; }

        private InteractionHandler currentGrabber;
        public InteractionHandler CurrentGrabber { get => currentGrabber; set => currentGrabber = value; }

        [SerializeField] private bool isGrabbed;
        public bool IsGrabbed { get => isGrabbed; set => isGrabbed = value; }

        private Rigidbody rigidBody;
        public Rigidbody RigidBody { get => rigidBody; set => rigidBody = value; }

        private Vector3 initialLocalScale;
        public Vector3 InitialLocalScale { get => initialLocalScale; set => initialLocalScale = value; }

        private Vector3 defaultPosition;
        private Quaternion defaultRotation;

        private bool defaultKinematicState;
        private bool defaultGravityState;

        private Transform defaultParent;
        private VelocityEstimator velocityEstimator;

        private Vector3 velocity;
        private Vector3 angularVelocity;
        private bool wasAttached;

        private InteractionHandler interactionHandler;

        private bool hasCollided;
        public InteractionHandler InteractionHandler { get => interactionHandler; set => interactionHandler = value; }

        #region Events

        public UnityEvent OnHover;
        public UnityEvent OnUnhover;
        public UnityEvent OnFirstAttach;
        public UnityEvent OnAttach;
        public UnityEvent OnHeld;
        public UnityEvent OnDetach;
        public UnityEvent OnReset;

        #endregion

        private void Start()
        {
            GetDefaultRigidbodyValues(RigidBody);
        }

        private void Awake()
        {
            if (grabbableModifier == null) grabbableModifier = gameObject.AddComponent<KinematicGrabbableModifier>();

            rigidBody = GetComponent<Rigidbody>();
            GetComponentsInChildren<GrabPoint>();
            GetDefaultTransformValues(transform);
            initialLocalScale = transform.localScale;
            velocityEstimator = GetComponent<VelocityEstimator>();
        }

        private void Update()
        {
            if (isGrabbed) OnHeld.Invoke();
        }

        /// <summary>
        /// Attaches the grabbable to the given grabParent and starts velocity estimation. Triggers OnAttach event.
        /// </summary>
        /// <param name="grabParent"></param>
        public virtual void Attach(Transform grabParent)
        {
            if (isGrabbed) return;

            StartEstimatingVelocity();

            currentGrabber = grabParent.GetComponent<InteractionHandler>();
            interactionHandler = currentGrabber;

            if (!wasAttached)
            {
                OnFirstAttach.Invoke();
                OnAttach.Invoke();
                wasAttached = true;
            }
            else
            {
                OnAttach.Invoke();
            }

            grabbableModifier.Attach(grabParent);
        }

        /// <summary>
        /// Detaches the grabbable from the object currently grabbing it and stops velocity estimation. Triggers OnDetach event.
        /// </summary>
        public virtual void Detach()
        {
            StopEstimatingVelocity();

            grabbableModifier.Detach();
        }

        private void StartEstimatingVelocity()
        {
            if (velocityEstimator) velocityEstimator.StartVelocityEstimation();
        }

        private void StopEstimatingVelocity()
        {
            if (velocityEstimator != null)
            {
                VelocityEstimatorController.GetReleaseVelocities(velocityEstimator, out velocity, out angularVelocity);
                VelocityEstimatorController.SetReleaseVelocities(RigidBody, velocity, angularVelocity);
                velocityEstimator.FinishVelocityEstimation();
            }
        }

        private void GetDefaultTransformValues(Transform transform)
        {
            defaultPosition = transform.position;
            defaultRotation = transform.rotation;
        }

        public void SetDefaultTransformValues()
        {
            transform.position = defaultPosition;
            transform.rotation = defaultRotation;
        }

        public void GetDefaultRigidbodyValues(Rigidbody rigidbody)
        {
            defaultKinematicState = rigidbody.isKinematic;
            defaultGravityState = rigidbody.useGravity;
            defaultParent = transform.parent;
        }

        public void SetDefaultRigidbodyValues(Rigidbody rigidbody, Transform transform)
        {
            rigidbody.isKinematic = defaultKinematicState;
            rigidbody.useGravity = defaultGravityState;
            transform.parent = defaultParent;
        }

        public void SetNewRigidbodyValues(Rigidbody rigidbody)
        {
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isRespawnable) return;
            var reset = collision.gameObject.GetComponent<ResetObject>();
            if (!reset) return;

            if (reset && !hasCollided)
            {
                hasCollided = true;
                var respawnObject = RespawnObject(reset.RespawnTime, reset.PrefabSpawner, reset.Particle);
                StartCoroutine(respawnObject);
            }
        }

        public IEnumerator RespawnObject(float respawnTime, PrefabSpawner prefabSpawner, GameObject particle)
        {
            yield return new WaitForSeconds(respawnTime);
            if (IsGrabbed)
            {
                hasCollided = false;
                yield break;
            }

            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;

            SetDefaultTransformValues();
            SetDefaultRigidbodyValues(RigidBody, transform);

            prefabSpawner.SpawnPrefabAtTransform(particle, transform, null, 0.5f);
            hasCollided = false;
            OnReset.Invoke();
        }
    }
}
