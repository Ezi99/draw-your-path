using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    public class RuntimeGrabPointCreator : MonoBehaviour
    {
        [SerializeField] private HandSkeleton handSkeleton;
        [SerializeField] private KeyCode saveKey = KeyCode.Space;
        [SerializeField] private PoseData poseData;
        [SerializeField] private PoseableHand poseableHand;
        
        [HideInInspector] [SerializeField] private Grabbable currentGrabbable;
    
        private void OnTriggerEnter(Collider other)
        {
            currentGrabbable = other.GetComponent<Grabbable>();
        }

        private void OnTriggerStay(Collider other)
        {
            currentGrabbable = other.GetComponent<Grabbable>();
        
            if(!currentGrabbable) return;
        
            if (UnityEngine.Input.GetKeyDown(saveKey))
            {
                var s = Instantiate(new GameObject());
                s.transform.localPosition = transform.position;
                s.transform.localRotation = transform.rotation;
                s.transform.SetParent(other.transform);
            
                var gp = s.AddComponent<GrabPoint>();
                gp.handType = handSkeleton.HandType;

                gp.name = $"GrabPoint_{handSkeleton.HandType.ToString().Substring(0, 1)}";
            
                poseData.SaveInRuntime(poseableHand);
            
                CustomHandPose customHandPose = new CustomHandPose();
                customHandPose.SetHandPoseData(poseData);

                gp.grabPose = customHandPose;
            
                gp.InstantiateHandPose();
            }
        }
    }    
}
