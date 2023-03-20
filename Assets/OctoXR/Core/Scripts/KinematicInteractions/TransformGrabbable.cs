using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    public class TransformGrabbable : Grabbable
    {
        /*
        [SerializeField] private SoundPlayer soundPlayer;

        [SerializeField] private float minimumScaleFactor = 0.5f;
        public float MinimumScaleFactor { get => minimumScaleFactor; set => minimumScaleFactor = value; }
        [SerializeField] private float maximumScaleFactor = 1.5f;
        public float MaximumScaleFactor { get => maximumScaleFactor; set => maximumScaleFactor = value; }
        [SerializeField] private float scaleDistanceFactor = 2f;
        public float ScaleDistanceFactor { get => scaleDistanceFactor; set => scaleDistanceFactor = value; }

        private int grabberCount;
        private Transform firstPoint;
        public Transform FirstPoint { get => firstPoint; set => firstPoint = value; }

        private Transform secondPoint;
        public Transform SecondPoint { get => secondPoint; set => secondPoint = value; }

        private GrabInteractor firstGrabber;
        public GrabInteractor FirstGrabber { get => firstGrabber; set => firstGrabber = value; }

        private GrabInteractor secondGrabber;
        public GrabInteractor SecondGrabber { get => secondGrabber; set => secondGrabber = value; }

        private bool isScaling;
        public bool IsScaling { get => isScaling; set => isScaling = value; }

        private bool isRotating;
        public bool IsRotating { get => isRotating; set => isRotating = value; }

        private bool isMoving;
        public bool IsMoving { get => isMoving; set => isMoving = value; }

        private float grabDistance;
        public float GrabDistance { get => grabDistance; set => grabDistance = value; }

        private float pinchDelta;
        public float PinchDelta { get => pinchDelta; set => pinchDelta = value; }

        private float lastFrameScale;
        public float LastFrameScale { get => lastFrameScale; set => lastFrameScale = value; }

        private Rigidbody baseRigidbody;
        public Rigidbody BaseRigidbody { get => baseRigidbody; set => baseRigidbody = value; }

        private GameObject positionAnchor;
        public GameObject PositionAnchor { get => positionAnchor; set => positionAnchor = value; }

        [HideInInspector][SerializeField] private ManipulationBase manipulationBase;

        private void Start()
        {
            IsPrecisionGrab = true;
            baseRigidbody = gameObject.GetComponent<Rigidbody>();
            SetPositionAnchor();
        }

        private void SetPositionAnchor()
        {
            if (positionAnchor) return;
            positionAnchor = new GameObject();
            positionAnchor.transform.SetParent(transform);
            positionAnchor.name = "PositionAnchor";
        }

        public override void Attach(Transform grabParent)
        {
            base.Attach(grabParent);

            HandleAttaching(grabParent);
        }

        private void HandleAttaching(Transform grabParent)
        {
            grabberCount++;

            if (grabberCount > 0 && grabberCount < 2)
            {
                FirstAttach(grabParent);
            }

            if (grabberCount >= 2)
            {
                SecondAttach(grabParent);
            }
        }

        private void FirstAttach(Transform grabParent)
        {
            transform.SetParent(grabParent);
            grabParent.localRotation = Quaternion.Euler(0, 0, 0);
            firstGrabber = grabParent.GetComponent<GrabInteractor>();
            firstPoint = grabParent;
        }

        private void SecondAttach(Transform grabParent)
        {
            secondGrabber = grabParent.GetComponent<GrabInteractor>();
            secondPoint = grabParent;

            grabDistance = (firstPoint.position - secondPoint.position).magnitude;

            transform.SetParent(null);

            if (!isMoving) manipulationBase.PositionManipulator.Start(this);
            if (!isRotating) manipulationBase.RotationManipulator.Start(this);
            if (!isScaling) manipulationBase.ScaleManipulator.Start(this);
        }

        private void Update()
        {
            if (isScaling)
            {
                grabDistance = (firstPoint.position - secondPoint.position).magnitude;
                pinchDelta = grabDistance - manipulationBase.ScaleManipulator.ScaleStartingDistance;
                manipulationBase.ScaleManipulator.Update(this);
                soundPlayer.PlayAudio(this);
            }

            if (isRotating) manipulationBase.RotationManipulator.Update(this);

            if (isMoving) manipulationBase.PositionManipulator.Update(this);
        }

        public override void Detach()
        {
            base.Detach();

            HandleDetach();
        }

        private void HandleDetach()
        {
            grabberCount--;

            if (grabberCount > 0 && grabberCount <= 2)
            {
                FirstDetach();
            }

            if (grabberCount == 0)
            {
                SecondDetach();
            }

            RigidBody.isKinematic = true;

            manipulationBase.PositionManipulator.Stop(this);
            manipulationBase.ScaleManipulator.Stop(this);
            manipulationBase.RotationManipulator.Stop(this);
        }

        private void FirstDetach()
        {
            if (!firstGrabber.InteractionHand.IsGrabbing)
            {
                firstGrabber = secondGrabber;
                secondGrabber = null;
                firstPoint = firstGrabber.transform;
                secondPoint = null;
            }

            transform.SetParent(firstPoint);
        }

        private void SecondDetach()
        {
            transform.SetParent(null);
            IsGrabbed = false;

            firstGrabber = null;
            secondGrabber = null;
            firstPoint = null;
            secondPoint = null;
        }
    */
    }
}