using UnityEngine;

namespace OctoXR.KinematicInteractions.Utilities
{
    /// <summary>
    /// Controls the color and scale of a given mesh renderer. These methods are meant to be used in conjunction with events from Grabbable objects. 
    /// </summary>
    public class MeshRendererInteractionIndicator : InteractionIndicator
    {
        [Tooltip("Reference to the mesh renderer whose color and scale you want to change.")]
        [SerializeField] private MeshRenderer meshRenderer;

        public override void Start()
        {
            base.Start();
            startingColor = meshRenderer.material.color;
        }

        public override void ChangeColor()
        {
            base.ChangeColor();
            meshRenderer.material.color = interactionColor;
        }

        public override void RevertColor()
        {
            base.RevertColor();
            meshRenderer.material.color = startingColor;
        }
    }
}
