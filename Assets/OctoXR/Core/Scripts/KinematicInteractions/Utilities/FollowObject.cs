using System;
using UnityEngine;

namespace OctoXR.KinematicInteractions.Utilities
{
    public class FollowObject : MonoBehaviour
    {
        [Tooltip("Object whose position this object should follow.")]
        [SerializeField] private Transform positionParent;

        [Tooltip("Determines how much the object should be offset from its position parent, leaving zero at all axes will result in the object following the exact position of its parent.")]
        [SerializeField] private Vector3 positionOffset;

        [Tooltip("Object whose rotation this object should follow.")]
        [SerializeField] private Transform rotationParent;

        [Tooltip("Line renderer which the curve should be drawn from.")]
        [SerializeField] private LineRenderer lineRenderer;

        [Tooltip("Starting position of the curve.")]
        [SerializeField] private Transform curveStart;

        [Tooltip("Determines how much the start of the curve should be offset from its position parent, leaving zero at all axes will result in the curve starting from the given origin point.")]
        [SerializeField] private Vector3 curveOffset;

        [Tooltip("Determines whether the object will follow its positional and rotational parents at all times or just at start.")]
        [SerializeField] private bool isConstantUpdate;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            if (isConstantUpdate)
            {
                if (rotationParent) FollowRotation();
                if (positionParent) FollowPosition();

                if (curveStart)
                    RaycastVisuals.DrawCurve(curveStart.position,
                    transform.position + new Vector3(curveOffset.x, curveOffset.y, 0), lineRenderer);
            }
        }

        private void FollowRotation()
        {
            var relativePos = rotationParent.position - transform.position;

            if (relativePos == Vector3.zero) return;

            var rot = Quaternion.LookRotation(-relativePos, Vector3.up);

            transform.rotation = rot;
        }

        private void FollowPosition()
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(positionParent.transform.position.x + positionOffset.x,
                    positionParent.transform.position.y + positionOffset.y,
                    positionParent.transform.position.z + positionOffset.z), 660f * Time.deltaTime);
        }

        private void OnEnable()
        {
            if (!isConstantUpdate)
            {
                transform.SetPositionAndRotation(positionParent.position, positionParent.rotation);
            }
        }
    }
}
