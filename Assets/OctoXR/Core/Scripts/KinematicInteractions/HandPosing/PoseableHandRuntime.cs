namespace OctoXR.KinematicInteractions
{
    public class PoseableHandRuntime : PoseableHand
    {
        private void OnEnable()
        {
            Joints = CollectJoints();
        }
    }
}
