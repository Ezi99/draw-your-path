namespace OctoXR.Rendering
{
    public class VisualizedHandSkeleton : HandSkeleton
    {
        /// <summary>
        /// Calls base <see cref="HandSkeleton.UpdatePose"/>, then <see cref="HandSkeleton.SetBoneBindPoses"/>
        /// and raises <see cref="HandSkeleton.OnPoseUpdated"/> event
        /// </summary>
        protected virtual void LateUpdate()
        {
            UpdatePose();
            SetBoneBindPoses();
            FinalizePoseUpdate();
        }
    }
}
