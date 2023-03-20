using UnityEngine;

namespace OctoXR.HandPoseDetection
{
    public class HandPoseOrientation : MonoBehaviour, IHandPoseComponent
    {
        [SerializeField] private PoseOrientation handPoseOrientation;
        [Tooltip("Orientation tolerance.\n" +
            "0 = Easier to detect (sometimes wrong).\n" +
            "1 = Precise detection (hard to get).")]
        [SerializeField, Range(0, 1)] private float threshold = 0.7f;

        private bool shouldCheckPalmOrientation;
        private bool shouldCheckFingerOrientation;
        private bool shouldCheckPalmUpDown;
        private bool shouldCheckPalmForwardBack;
        private bool shouldCheckPalmRightLeft;
        private bool shouldCheckFingersUpDown;
        private bool shouldCheckFingersForward;
        private bool shouldCheckFingersRightLeft;

        private Transform handSkeleton;
        private static Transform _camera;
        private static PoseOrientation palmOrientation;
        private static PoseOrientation fingersOrientation;

        public PoseOrientation PoseOrientation
        {
            get => handPoseOrientation;
            set
            { 
                handPoseOrientation = value;
                UpdateHandState();
            }
        }

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main.transform;
            }

            if (palmOrientation == 0)
            {
                palmOrientation = PoseOrientation.PalmUp | PoseOrientation.PalmDown |
                    PoseOrientation.PalmForward | PoseOrientation.PalmBack |
                    PoseOrientation.PalmRight | PoseOrientation.PalmLeft;
            }

            if (fingersOrientation == 0)
            {
                fingersOrientation = PoseOrientation.FingersUp | PoseOrientation.FingersDown |
                    PoseOrientation.FingersRight | PoseOrientation.FingersLeft |
                    PoseOrientation.FingersForward;
            }

            UpdateHandState();
        }

        private void UpdateHandState()
        {
            shouldCheckPalmOrientation = (handPoseOrientation & palmOrientation) != 0;
            shouldCheckFingerOrientation = (handPoseOrientation & fingersOrientation) != 0;

            if (shouldCheckPalmOrientation)
            {
                shouldCheckPalmUpDown = (handPoseOrientation & PoseOrientation.PalmUp) != 0 || (handPoseOrientation & PoseOrientation.PalmDown) != 0;
                shouldCheckPalmForwardBack = (handPoseOrientation & PoseOrientation.PalmForward) != 0 || (handPoseOrientation & PoseOrientation.PalmBack) != 0;
                shouldCheckPalmRightLeft = (handPoseOrientation & PoseOrientation.PalmRight) != 0 || (handPoseOrientation & PoseOrientation.PalmLeft) != 0;
            }

            if (shouldCheckFingerOrientation)
            {
                shouldCheckFingersUpDown = (handPoseOrientation & PoseOrientation.FingersUp) != 0 || (handPoseOrientation & PoseOrientation.FingersDown) != 0;
                shouldCheckFingersRightLeft = (handPoseOrientation & PoseOrientation.FingersRight) != 0 || (handPoseOrientation & PoseOrientation.FingersLeft) != 0;
                shouldCheckFingersForward = (handPoseOrientation & PoseOrientation.FingersForward) != 0;
            }
        }

        public bool Detect()
        {
            var isOrientationDetected = false;

            if (shouldCheckPalmOrientation && shouldCheckFingerOrientation)
            {
                isOrientationDetected = DetectPalmOrientation() && DetectFingerOrientation();
            }
            else if (shouldCheckPalmOrientation)
            {
                isOrientationDetected = DetectPalmOrientation();
            }
            else if (shouldCheckFingerOrientation)
            {
                isOrientationDetected = DetectFingerOrientation();
            }

            return isOrientationDetected;
        }

        private bool DetectPalmOrientation()
        {
            PoseOrientation currentPoseOrientation = 0;

            if (shouldCheckPalmUpDown)
            {
                var dotPalmForwardAndUp = Vector3.Dot(handSkeleton.forward, Vector3.up);

                if (dotPalmForwardAndUp > threshold)
                {
                    currentPoseOrientation |= PoseOrientation.PalmUp;
                }
                else if (dotPalmForwardAndUp < -threshold)
                {
                    currentPoseOrientation |= PoseOrientation.PalmDown;
                }
            }

            if (shouldCheckPalmForwardBack)
            {
                var cameraForwardProjection = Vector3.ProjectOnPlane(_camera.forward, Vector3.up);
                var dotCameraAndPalmForward = Vector3.Dot(cameraForwardProjection, handSkeleton.forward);

                if (dotCameraAndPalmForward > threshold)
                {
                    currentPoseOrientation |= PoseOrientation.PalmForward;
                }
                else if (dotCameraAndPalmForward < -threshold)
                {
                    currentPoseOrientation |= PoseOrientation.PalmBack;
                }
            }

            if (shouldCheckPalmRightLeft)
            {
                var cameraRightProjection = Vector3.ProjectOnPlane(_camera.right, Vector3.up);
                var dotCameraRightAndPalmForward = Vector3.Dot(cameraRightProjection, handSkeleton.forward);

                if (dotCameraRightAndPalmForward > threshold)
                {
                    currentPoseOrientation |= PoseOrientation.PalmRight;
                }
                else if (dotCameraRightAndPalmForward < -threshold)
                {
                    currentPoseOrientation |= PoseOrientation.PalmLeft;
                }
            }

            return (handPoseOrientation & currentPoseOrientation) != 0;
        }

        private bool DetectFingerOrientation()
        {
            PoseOrientation currentPoseOrientation = 0;

            if (shouldCheckFingersUpDown)
            {
                var dotFingersUpAndDown = Vector3.Dot(handSkeleton.up, Vector3.up);

                if (dotFingersUpAndDown > threshold)
                {
                    currentPoseOrientation |= PoseOrientation.FingersUp;
                }
                else if (dotFingersUpAndDown < -threshold)
                {
                    currentPoseOrientation |= PoseOrientation.FingersDown;
                }
            }

            if (shouldCheckFingersForward)
            {
                var cameraForwardProjection = Vector3.ProjectOnPlane(_camera.forward, Vector3.up);
                var dotFingersForward = Vector3.Dot(cameraForwardProjection, handSkeleton.up);

                if (dotFingersForward > threshold)
                { 
                    currentPoseOrientation |= PoseOrientation.FingersForward;
                }
            }

            if (shouldCheckFingersRightLeft)
            {
                var cameraRightProjection = Vector3.ProjectOnPlane(_camera.right, Vector3.up);
                var dotCameraRightAndPalmUp = Vector3.Dot(cameraRightProjection, handSkeleton.up);

                if (dotCameraRightAndPalmUp > threshold)
                {
                    currentPoseOrientation |= PoseOrientation.FingersRight;
                }
                else if (dotCameraRightAndPalmUp < -threshold)
                {
                    currentPoseOrientation |= PoseOrientation.FingersLeft;
                }
            }

            return (handPoseOrientation & currentPoseOrientation) != 0;
        }

        public void InjectHandSkeleton(HandSkeleton handSkeleton)
        {
            this.handSkeleton = handSkeleton.Transform;
        }
    }
}
