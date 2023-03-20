using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Abstract class which can be used to create different types of interaction hand drivers responsible for grab detection.
    /// </summary>
    /// <param name="InteractionHandDriver"></param>

    public abstract class InteractionHandDriver : MonoBehaviour
    {
        private InteractionHand interactionHand;
        public InteractionHand InteractionHand { get => interactionHand; set => interactionHand = value; }

        private void Awake()
        {
            interactionHand = GetComponent<InteractionHand>();
        }

        private void Update()
        {
            DetectGrab();
        }

        public virtual void DetectGrab(){}
    }
}
