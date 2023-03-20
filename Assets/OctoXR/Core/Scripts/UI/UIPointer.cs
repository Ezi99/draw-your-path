using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OctoXR.UI
{
    public class UIPointer : MonoBehaviour
    {
        [SerializeField] protected HandSkeleton handSkeleton;
        [SerializeField] protected Transform palmCenter;
        [SerializeField] private RayVisuals rayVisuals;
        [SerializeField] private Transform lineStart;
        [SerializeField] private GameObject pointer;
        [SerializeField] private GameObject pointerSelected;
        [Tooltip("Button press/pinch strength after which select action will be registered.")]
        [SerializeField][Range(0, 1f)] private float selectActionThreshold = 0.7f;
        [Tooltip("Game objects on these layers will be ignored and UI raycast will pass through them.")]
        [SerializeField] private LayerMask ignoreLayer;

        [Header("Events")]
        public UnityEvent OnStartSelect;
        public UnityEvent OnStopSelect;
        public UnityEvent OnPointerEnterCanvas;
        public UnityEvent OnPointerExitCanvas;

        private OctoInputModule octoInputModule;
        private Transform indexFingerTip;
        private Transform thumbFingerTip;
        private Vector3 rayDirection;
        private LayerMask layerMask;
        private float startingPointerScale;
        private int pointerIndex;
        private bool hover = false;
        private bool wasPinching = false;
        private IPointer[] pointers;
        private IPointer currentActivePointer;

        private static Camera CanvasCamera;
        private static Transform canvasCameraTransform;

        private void Awake()
        {
            SetupOctoInputModule();
            CacheFields();
        }

        private void SetupOctoInputModule()
        {
            octoInputModule = FindObjectOfType<OctoInputModule>();

            if (octoInputModule == null)
            {
                var eventSystem = FindObjectOfType<EventSystem>();

                if (eventSystem != null)
                {
                    octoInputModule = eventSystem.gameObject.AddComponent<OctoInputModule>();
                }
                else
                {
                    Debug.LogError("OctoInputModule not found in the scene!");
                }
            }
        }

        private void CacheFields()
        {
            indexFingerTip = handSkeleton.Bones[HandBoneId.IndexFingerTip].Transform;
            thumbFingerTip = handSkeleton.Bones[HandBoneId.ThumbFingerTip].Transform;

            startingPointerScale = pointer.transform.localScale.x;

            var layers = new string[] { Constants.Hand, Constants.OctoPlayer, Constants.IgnoreRaycast };
            layerMask = ignoreLayer | LayerMask.GetMask(layers);

            if (!palmCenter && handSkeleton)
            {
                palmCenter = handSkeleton.Transform.Find("Marker_PalmCenter");
            }

            pointers = GetComponentsInChildren<IPointer>();

            foreach (var pointer in pointers)
            {
                pointer.InjectPalmCenter(palmCenter);
            }
        }

        private void OnEnable()
        {
            if (CanvasCamera == null)
            {
                CanvasCamera = new GameObject("Canvas Camera").AddComponent<Camera>();
                CanvasCamera.clearFlags = CameraClearFlags.Nothing;
                CanvasCamera.stereoTargetEye = StereoTargetEyeMask.None;
                CanvasCamera.orthographic = true;
                CanvasCamera.orthographicSize = 0.001f;
                CanvasCamera.cullingMask = 0;
                CanvasCamera.nearClipPlane = 0.01f;
                CanvasCamera.depth = 0f;
                CanvasCamera.allowHDR = false;
                CanvasCamera.enabled = false;
                CanvasCamera.fieldOfView = 0.00001f;
                canvasCameraTransform = CanvasCamera.transform;
            }

            UpdateCanvases();
        }

        /// <summary>
        /// If you create canvas at runtime, call this method to update the pointer canvases list
        /// </summary>
        public void UpdateCanvases()
        {
            var canvases = FindObjectsOfType<Canvas>();

            for (var i = 0; i < canvases.Length; i++)
            {
                canvases[i].worldCamera = CanvasCamera;
            }

            if (octoInputModule.Instance != null)
            {
                pointerIndex = octoInputModule.Instance.AddPointer(this);
            }
        }

        private void OnDisable()
        {
            if (octoInputModule.Instance != null)
            {
                octoInputModule.Instance.RemovePointer(this);
            }
        }

        private void Update()
        {
            foreach (var pointer in pointers)
            {
                if (pointer.IsProviderTracking)
                {
                    currentActivePointer = pointer;
                    break;
                }
            }

            if (currentActivePointer == null) return;

            lineStart.position = (indexFingerTip.position + thumbFingerTip.position) / 2;

            PointerEventData pointerEventData = octoInputModule.GetPointerEventData(pointerIndex);
            float canvasDistance = pointerEventData.pointerCurrentRaycast.distance;

            PointerCanvasDetection(canvasDistance);

            var selectStrength = currentActivePointer.GetSelectActionStrength();

            var raycastHit = CreateRaycast(canvasDistance);

            PointerActionDetection(selectStrength, raycastHit);

            if (!hover || raycastHit.collider)
            {
                HideRay();
                return;
            }

            UpdatePointerVisuals(selectStrength, canvasDistance, pointerEventData.pointerCurrentRaycast.worldNormal);
        }

        private void PointerCanvasDetection(float canvasDistance)
        {
            if (canvasDistance != 0 && !hover)
            {
                OnPointerEnterCanvas?.Invoke();
                hover = true;
            }
            else if (canvasDistance == 0 && hover)
            {
                OnPointerExitCanvas?.Invoke();
                hover = false;
            }
        }

        private RaycastHit CreateRaycast(float distance)
        {
            rayDirection = currentActivePointer.CalculateRayDirection();
            Ray ray = new Ray(palmCenter.position, rayDirection);
            UnityEngine.Physics.Raycast(ray, out RaycastHit raycastHit, distance, layerMask);

            return raycastHit;
        }

        private void PointerActionDetection(float selectStrength, RaycastHit raycastHit)
        {
            if (selectStrength > selectActionThreshold)
            {
                if (pointer.activeSelf)
                {
                    pointer.SetActive(false);
                    pointerSelected.SetActive(true);
                }

                if (!wasPinching && hover && !raycastHit.collider)
                {
                    Select();
                    wasPinching = true;
                }
            }
            else
            {
                if (pointerSelected.activeSelf)
                {
                    pointerSelected.SetActive(false);
                    pointer.SetActive(true);
                }

                if (wasPinching)
                {
                    if (raycastHit.collider)
                    {
                        Cancel();
                    }
                    else
                    {
                        Release();
                    }

                    wasPinching = false;
                }
            }
        }

        private void HideRay()
        {
            pointer.SetActive(false);
            pointerSelected.SetActive(false);
            pointer.transform.rotation = Quaternion.identity;
            pointerSelected.transform.rotation = Quaternion.identity;
            rayVisuals.DisableLineRenderer();
        }

        private void UpdatePointerVisuals(float pinchStrength, float canvasDistance, Vector3 canvasNormal)
        {
            if (-pointer.transform.forward != canvasNormal)
            {
                var pointerRotation = Quaternion.FromToRotation(-pointer.transform.forward, canvasNormal);
                pointer.transform.rotation = pointerRotation;
                pointerSelected.transform.rotation = pointerRotation;
            }

            var endPosition = lineStart.position + (rayDirection * canvasDistance);
            pointer.transform.position = endPosition;
            pointerSelected.transform.position = endPosition;

            var inversePinchStrength = 1 - pinchStrength;
            var scaledInversePinchStrength = inversePinchStrength * startingPointerScale;

            pointer.transform.localScale = new Vector3(scaledInversePinchStrength, scaledInversePinchStrength, scaledInversePinchStrength);

            if (!pointer.activeSelf)
            {
                pointer.SetActive(true);
            }

            rayVisuals.ReduceLineWidthByPercentage(pinchStrength);

            if (pinchStrength > selectActionThreshold)
            {
                rayVisuals.DrawSelectRay(lineStart.position, endPosition);
            }
            else
            {
                rayVisuals.DrawValidRay(lineStart.position, endPosition);
            }
        }

        private void Select()
        {
            octoInputModule.ProcessSelect(pointerIndex);
            OnStartSelect?.Invoke();
        }

        private void Release()
        {
            octoInputModule.ProcessRelease(pointerIndex);
            OnStopSelect?.Invoke();
        }

        private void Cancel()
        {
            octoInputModule.ProcessCancel(pointerIndex);
        }

        internal virtual void Preprocess()
        {
            if (currentActivePointer == null) return;

            canvasCameraTransform.position = lineStart.position;
            rayDirection = currentActivePointer.CalculateRayDirection();
            canvasCameraTransform.forward = rayDirection;
        }

        public void SetIndex(int index)
        {
            pointerIndex = index;
        }
    }
}
