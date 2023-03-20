using UnityEngine;

namespace OctoXR.KinematicInteractions.Utilities
{
    /// <summary>
    /// Controls the color and scale of a given interactor.
    /// This is not actually used in scenes, but serves as an abstract class to be inherited from. 
    /// </summary>
    public abstract class InteractionIndicator : MonoBehaviour
    {
        [Tooltip("Color upon interaction.")]
        public Color32 interactionColor;
        [Tooltip("How much the interaction indicator will scale upon interaction. Leave this at 0 if you dont want to scale the interaction indicator.")]
        public float scaleFactor;

        [HideInInspector] public Color startingColor;
        [HideInInspector] public Vector3 startingScale;
        [HideInInspector] public bool hasEntered;

        public virtual void Start()
        {
            startingScale = transform.localScale;
        }

        /// <summary>
        /// Changes the color and scale of the interaction indicator to the color set by interactionColor and scaleFactor variables.
        /// </summary>
        public virtual void ChangeColor()
        {
            if (hasEntered) return;
            transform.localScale = startingScale + new Vector3(scaleFactor, scaleFactor, scaleFactor);
            hasEntered = true;
        }


        /// <summary>
        /// Reverts the color and scale back to the interactor's original values.
        /// </summary>
        public virtual void RevertColor()
        {
            if (!hasEntered) return;
            transform.localScale = startingScale;
            hasEntered = false;
        }
    }
}
