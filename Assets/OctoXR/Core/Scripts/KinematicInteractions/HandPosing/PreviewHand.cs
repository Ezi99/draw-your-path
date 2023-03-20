using System;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Implementation of the poseable hand used in the editor. Applies pose (if any exist) to the preview hand.
    /// </summary>
    [SelectionBase]
    [ExecuteInEditMode]
    public class PreviewHand : EditorPoseableHand
    {
        private void OnEnable()
        {
            Joints = CollectJoints();
            var CurrentGrabPoint = GetComponentInParent<GrabPoint>();
            CurrentHandPose = CurrentGrabPoint.GrabPose;
            if (CurrentHandPose) ApplyPose(CurrentHandPose, CurrentGrabPoint.IsPoseInverted);
        }
    }
}
