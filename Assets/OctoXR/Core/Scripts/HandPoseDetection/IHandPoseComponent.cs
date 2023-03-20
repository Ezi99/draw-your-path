namespace OctoXR.HandPoseDetection
{
    public interface IHandPoseComponent
    {
        public bool Detect();
        public void InjectHandSkeleton(HandSkeleton handSkeleton);
    }
}
