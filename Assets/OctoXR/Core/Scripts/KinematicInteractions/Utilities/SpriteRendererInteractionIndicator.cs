using UnityEngine;

namespace OctoXR.KinematicInteractions.Utilities
{
    /// <summary>
    /// Controls the color and scale of a given sprite renderer. These methods are meant to be used in conjunction with events from Grabbable objects. 
    /// </summary>
    public class SpriteRendererInteractionIndicator : InteractionIndicator
    {
        [Tooltip("Reference to the sprite render whose color and scale you want to change.")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        public override void Start()
        {
            base.Start();
            startingColor = spriteRenderer.color;
        }

        public override void ChangeColor()
        {
            base.ChangeColor();
            spriteRenderer.color = interactionColor;
        }

        public override void RevertColor()
        {
            base.RevertColor();
            spriteRenderer.color = startingColor;
        }
    }
}
