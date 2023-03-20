using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.KinematicInteractions.Utilities
{
    public class InteractionVisuals : MonoBehaviour
    {
        [SerializeField] private InteractionHandler interactionHandler;
        [SerializeField] private GameObject visual;

        private Vector3 initialScale;

        private IEnumerator scaleUp;
        private IEnumerator scaleDown;
        private bool shouldScale;

        private float timeToScale = 0.5f;

        private void Start()
        {
            initialScale = transform.localScale;
            visual.SetActive(false);
            transform.parent = null;
            transform.localScale = Vector3.zero;

            scaleUp = ScaleObjectUp();
            scaleDown = ScaleObjectDown();
        }

        private void OnEnable()
        {
            interactionHandler.OnHover.AddListener(ActivateVisuals);
            interactionHandler.OnGrab.AddListener(DeactivateVisuals);
            interactionHandler.OnUnhover.AddListener(DeactivateVisuals);
        }

        private void OnDisable()
        {
            interactionHandler.OnHover.RemoveListener(ActivateVisuals);
            interactionHandler.OnUnhover.RemoveListener(DeactivateVisuals);
        }

        private void ActivateVisuals()
        {
            visual.SetActive(true);
            visual.transform.position = interactionHandler.ClosestObject.transform.position;

            StartCoroutine(ScaleObjectUp());

            visual.transform.SetParent(interactionHandler.ClosestObject.transform);
        }

        private IEnumerator ScaleObjectUp()
        {
            float time = 0;

            while (time < timeToScale)
            {
                visual.transform.localScale = Vector3.Lerp(visual.transform.localScale, initialScale, time / timeToScale);
                time += Time.deltaTime;
                yield return null;
            }

            visual.transform.localScale = initialScale;
        }

        private IEnumerator ScaleObjectDown()
        {
            float time = 0;

            while (time < timeToScale)
            {
                visual.transform.localScale = Vector3.Lerp(visual.transform.localScale, Vector3.zero, time / timeToScale);
                time += Time.deltaTime;
                yield return null;
            }

            visual.transform.localScale = Vector3.zero;
        }

        private void DeactivateVisuals()
        {
            StartCoroutine(ScaleObjectDown());
            visual.transform.parent = null;
        }
    }
}
