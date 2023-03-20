using OctoXR.Samples.KinematicInteractions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OctoXR.KinematicInteractions.Utilities
{
    /// <summary>
    /// Used mainly for UI interactions. 
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class HoldToActivate : MonoBehaviour
    {
        [Tooltip("How long the colliders should overlap for the events to trigger.")]
        [SerializeField] private float holdTime;
        [Tooltip("Image with a radial fill which visually indicates the progress.")]
        [SerializeField] private Image progressImage;
        [Tooltip("Reference to the collider of the interactor.")]
        [SerializeField] private FingerInteractor fingerInteractor;
        [Tooltip("Audio source from which a sound should be played on interaction. Don't forget to add a clip to it.")]
        [SerializeField] private AudioSource audioSource;

        public UnityEvent OnHoldCompleted;
        public UnityEvent OnHoldCancelled;
        public UnityEvent OnHoldStart;

        private float currentTime;
        private bool doOnce = true;
        private bool areCollidersOverlapping = false;
        private bool wasHolding = false;
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnEnable()
        {
            currentTime = 0;
            doOnce = true;
            progressImage.enabled = false;
            progressImage.fillAmount = 0;
        }

        private void Update()
        {
            areCollidersOverlapping = UnityEngine.Physics.ComputePenetration(_collider, transform.position, transform.rotation,
                fingerInteractor.GetComponent<Collider>(), fingerInteractor.transform.position, fingerInteractor.transform.rotation, out _, out _);

            if (areCollidersOverlapping)
            {
                if (!doOnce) return;

                if (!wasHolding)
                {
                    OnHoldStart?.Invoke();
                    if (audioSource) audioSource.Play();
                    wasHolding = true;
                }

                progressImage.enabled = true;
                progressImage.fillAmount = currentTime / holdTime;

                currentTime += Time.unscaledDeltaTime;

                if (currentTime >= holdTime)
                {
                    OnHoldCompleted?.Invoke();
                    if (audioSource) audioSource.Stop();
                    doOnce = false;
                    currentTime = 0;
                    progressImage.enabled = false;
                    progressImage.fillAmount = 0;
                }
            }
            else
            {
                currentTime = 0;
                doOnce = true;
                progressImage.enabled = false;
                progressImage.fillAmount = 0;

                if (wasHolding)
                {
                    OnHoldCancelled?.Invoke();
                    if (audioSource) audioSource.Stop();
                    wasHolding = false;
                }
            }
        }
    }
}
