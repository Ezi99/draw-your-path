using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace OctoXR
{
    [RequireComponent(typeof(Camera))]
    public class ScreenFader : MonoBehaviour
    {
        public float FadeInSpeed = 8;
        public float FadeOutSpeed = 8;
        [SerializeField] private Material screenFaderMaterial;
        [SerializeField] private Color fadeColor = new Color(0, 0, 0);
        [SerializeField] private bool fadeOutOnStart;

        [Header("Events")]
        public UnityEvent OnFadeInStart;
        public UnityEvent OnFadeInEnd;
        public UnityEvent OnFadeOutStart;
        public UnityEvent OnFadeOutEnd;

        private GameObject canvasGameObject;
        private CanvasGroup canvasGroup;
        private static readonly string faderGameObjectName = "Screen Fader";
        private IEnumerator fadeRoutine;
        private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        public bool IsFadeInProgress { get; private set; }

        private void Awake()
        {
            InitializeCanvas();
            IsFadeInProgress = false;
        }

        private void InitializeCanvas()
        {
            if (canvasGameObject == null)
            {
                Canvas childCanvas = GetComponentInChildren<Canvas>();

                if (childCanvas != null && childCanvas.transform.name == faderGameObjectName)
                {
                    GameObject.Destroy(this.gameObject);
                    return;
                }
            }

            canvasGameObject = new GameObject(faderGameObjectName);
            canvasGameObject.transform.parent = transform;
            var canvas = canvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            var canvasRectTransform = canvas.GetComponent<RectTransform>();
            var camera = GetComponentInParent<Camera>();
            canvas.worldCamera = camera;
            var canvasOffset = camera.nearClipPlane + 0.0001f;
            canvasRectTransform.localPosition = new Vector3(0, 0, canvasOffset);
            canvasRectTransform.localRotation = Quaternion.identity;
            canvasRectTransform.sizeDelta = new Vector2(1920, 1080);
            var oneMeter = 1f / 1920f;
            canvasRectTransform.localScale = new Vector3(oneMeter, oneMeter, oneMeter);

            canvasGroup = canvasGameObject.AddComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;

            var image = canvasGameObject.AddComponent<Image>();
            var imageRectTransform = canvasGameObject.GetComponent<RectTransform>();
            imageRectTransform.anchorMin = new Vector2(0, 0);
            imageRectTransform.anchorMax = new Vector2(1, 1);
            image.color = fadeColor;
            image.raycastTarget = false;
            image.material = screenFaderMaterial;

            canvasGameObject.SetActive(false);
        }

        private void Start()
        {
            if (fadeOutOnStart)
            {
                canvasGroup.alpha = 1;
                DoFadeOut();
            }
            else
            {
                canvasGroup.alpha = 0;
            }
        }

        public void DoFadeIn()
        {
            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }

            if (canvasGroup != null)
            {
                fadeRoutine = DoFade(canvasGroup.alpha, 1);
                OnFadeInStart?.Invoke();
                StartCoroutine(fadeRoutine);
            }
        }

        public void DoFadeOut()
        {
            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }

            if (canvasGroup != null)
            {
                fadeRoutine = DoFade(canvasGroup.alpha, 0);
                OnFadeOutStart?.Invoke();
                StartCoroutine(fadeRoutine);
            }
        }

        private IEnumerator DoFade(float alphaFrom, float alphaTo)
        {
            IsFadeInProgress = true;
            float alpha = alphaFrom;

            UpdateImageAlpha(alpha);

            while (alpha != alphaTo)
            {
                if (alphaFrom < alphaTo)
                {
                    alpha += Time.deltaTime * FadeInSpeed;
                    if (alpha > alphaTo)
                    {
                        alpha = alphaTo;
                        OnFadeInEnd?.Invoke();
                    }
                }
                else
                {
                    alpha -= Time.deltaTime * FadeOutSpeed;
                    if (alpha < alphaTo)
                    {
                        alpha = alphaTo;
                        OnFadeOutEnd?.Invoke();
                    }
                }

                UpdateImageAlpha(alpha);

                yield return waitForEndOfFrame;
            }

            yield return waitForEndOfFrame;

            UpdateImageAlpha(alphaTo);
            IsFadeInProgress = false;
        }

        private void UpdateImageAlpha(float alphaValue)
        {
            if (canvasGroup == null) return;

            if (!canvasGameObject.activeSelf && alphaValue > 0)
            {
                canvasGameObject.SetActive(true);
            }

            canvasGroup.alpha = alphaValue;

            if (alphaValue == 0 && canvasGameObject.activeSelf)
            {
                canvasGameObject.SetActive(false);
            }
        }

        public float GetCanvasGroupAlphaValue() => canvasGroup.alpha;
    }
}
