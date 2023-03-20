using OctoXR.Input;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// An implementation of the InteractionHandDriver class used for grab detection while using controllers.
    /// </summary>
    /// <param name="ControllerInteractionHandDriver"></param>

    public class ControllerInteractionHandDriver : InteractionHandDriver
    {
        [SerializeField] private UnityXRControllerInputDataProvider unityXRControllerInputDataProvider;

        public override void DetectGrab()
        {
            InteractionHand.ShouldGrab = unityXRControllerInputDataProvider.Buttons.Grip.IsPressed;
        }
    }
}