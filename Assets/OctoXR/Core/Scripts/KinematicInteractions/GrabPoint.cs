using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Grab point of the object you create, contains and updates data about positional and rotational offset of its parent object.
    /// </summary>
    public class GrabPoint : MonoBehaviour
    {
        [Header("Hand options")]

        [Tooltip("Type of hand this grab point should look for")]
        [SerializeField] public HandType handType;
        public HandType HandType { get => handType; set => handType = value; }

        [SerializeField] private bool isPoseInverted;
        public bool IsPoseInverted { get => isPoseInverted; set => isPoseInverted = value; }

        [Tooltip("Grab pose that will be applied to the hand once it grabs this object")]
        [SerializeField] public CustomHandPose grabPose;
        public CustomHandPose GrabPose { get => grabPose; set => grabPose = value; }

        private Grabbable parentGrabbable;
        public Grabbable ParentGrabbable { get => parentGrabbable; set => parentGrabbable = value; }

        [SerializeField][HideInInspector] private Vector3 offset;
        public Vector3 Offset { get => offset; set => offset = value; }

        [SerializeField][HideInInspector] private bool isGrabbed;
        public bool IsGrabbed { get => isGrabbed; set => isGrabbed = value; }

        [SerializeField][HideInInspector] private Quaternion rotationOffset;
        public Quaternion RotationOffset { get => rotationOffset; set => rotationOffset = value; }

        [SerializeField][HideInInspector] private GameObject instantiatedPreviewHand;
        public GameObject InstantiatedPreviewHand { get => instantiatedPreviewHand; set => instantiatedPreviewHand = value; }

        private Object previewHandAsset;

        private Vector3 leftHandVector = new Vector3(-0.0042f, -0.0811f, -0.0189f);
        private Vector3 rightHandVector = new Vector3(0.0042f, -0.0811f, -0.0189f);

        private void Awake()
        {
            if (!parentGrabbable && transform.parent.GetComponent<Grabbable>())
            {
                parentGrabbable = transform.parent.GetComponent<Grabbable>();
            }
        }

        private void Start()
        {
            if (!parentGrabbable) parentGrabbable = transform.GetComponent<Grabbable>();
            UpdatePositionOffset();
            UpdateRotationOffset();
        }

        public void UpdatePositionOffset()
        {
            offset = Quaternion.Inverse(parentGrabbable.transform.rotation) *
                     (-parentGrabbable.transform.position + transform.position);
        }

        public void UpdateRotationOffset()
        {
            rotationOffset = Quaternion.Euler(0, 0, 0) * transform.localRotation;
        }

        /// <summary>
        /// Instantiates a preview hand with the pose set in grabPose, if there is no grab pose, the hand instantiates with an open palm.
        /// </summary>
        public void InstantiateHandPose()
        {
            GameObject handPose = null;

            if (handType == HandType.Left)
            {
                previewHandAsset = Resources.Load("PreviewHand_Left");

                handPose = (GameObject)previewHandAsset;
            }
            else if (handType == HandType.Right)
            {
                previewHandAsset = Resources.Load("PreviewHand_Right");

                handPose = (GameObject)previewHandAsset;
            }

            instantiatedPreviewHand = Instantiate(handPose, transform);

            if (handType == HandType.Left)
            {
                instantiatedPreviewHand.transform.localPosition = leftHandVector;
            }
            else if (handType == HandType.Right)
            {
                instantiatedPreviewHand.transform.localPosition = rightHandVector;
            }
        }
    }
}
