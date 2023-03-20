using System;
using System.Collections;
using UnityEngine;

namespace OctoXR.KinematicInteractions.Utilities
{
    public class FollowAnchor : MonoBehaviour
    {
        [SerializeField] private Transform centerEyeAnchor;
        [SerializeField] private Transform anchor;
        [SerializeField] private float distanceThreshold = 2f;

        private bool isPositionReached;
        [SerializeField] private bool calculatingDist;

        private float distance;

        private void OnEnable()
        {
            transform.position = new Vector3(anchor.transform.position.x, centerEyeAnchor.position.y,
                anchor.transform.position.z);
        }

        private void Update()
        {
            if (calculatingDist) distance = (anchor.transform.position - transform.position).magnitude;
            var finalPosition = new Vector3(anchor.transform.position.x, centerEyeAnchor.position.y,
                anchor.transform.position.z);
            float lerpDuration = 1;

            transform.LookAt(centerEyeAnchor.transform.position, Vector3.up);
            var rot = transform.rotation;
            rot.x = 0;
            rot.z = 0;

            transform.rotation = rot;

            if (distance > distanceThreshold)
            {
                calculatingDist = false;
                float time = 0;
                time += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, finalPosition, time / lerpDuration);
                if (transform.position == finalPosition) calculatingDist = true;
            }
        }
    }
}