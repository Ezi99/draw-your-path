using System.Collections;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Used to estimate release velocities of grabbable objects, 
    /// without it, the objects would behave as if no external forces 
    /// are impacting them.
    /// </summary>
    public class VelocityEstimator : MonoBehaviour
    {
        [Tooltip("Number of estimates taken to calculate average linear velocity.")]
        [SerializeField]
        private int linearVelocitySampleCount = 5;

        [Tooltip("Number of estimates taken to calculate average angular velocity.")]
        [SerializeField]
        private int angularVelocitySampleCount = 5;

        private int sampleCount;

        private Vector3[] linearVelocitySamples;
        private Vector3[] angularVelocitySamples;

        private Vector3 referencePosition;

        private Coroutine coroutine;

        private void Awake()
        {
            linearVelocitySamples = new Vector3[linearVelocitySampleCount];
            angularVelocitySamples = new Vector3[angularVelocitySampleCount];
        }

        /// <summary>
        ///     Starts estimating the velocity of the object.
        /// </summary>
        public void StartVelocityEstimation()
        {
            FinishVelocityEstimation();

            coroutine = StartCoroutine(EstimateVelocity());
        }

        /// <summary>
        ///     Stops velocity estimation.
        /// </summary>
        public void FinishVelocityEstimation()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        /// <summary>
        ///     Estimates the average linear velocity - the speed at which the thrown object should move.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLinearVelocityAverage()
        {
            var linearVelocityEstimate = Vector3.zero;
            linearVelocitySampleCount = Mathf.Min(sampleCount, linearVelocitySamples.Length);

            if (linearVelocitySampleCount != 0)
            {
                for (var i = 0; i < linearVelocitySamples.Length; i++)
                    linearVelocityEstimate += linearVelocitySamples[i];
                linearVelocityEstimate *= 1.0f / linearVelocitySampleCount;
            }

            return linearVelocityEstimate;
        }

        /// <summary>
        ///     Estimates the average angular velocity - the speed at which the thrown object should rotate.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetAngularVelocityAverage()
        {
            var angularVelocityEstimate = Vector3.zero;
            angularVelocitySampleCount = Mathf.Min(sampleCount, angularVelocitySamples.Length);

            if (angularVelocitySampleCount != 0)
            {
                for (var i = 0; i < angularVelocitySamples.Length; i++)
                    angularVelocityEstimate += angularVelocitySamples[i];
                angularVelocityEstimate *= 1.0f / angularVelocitySampleCount;
            }

            return angularVelocityEstimate;
        }

        private IEnumerator EstimateVelocity()
        {
            sampleCount = 0;

            var previousPosition = transform.position;
            var previousRotation = transform.rotation;
            while (true)
            {
                yield return new WaitForEndOfFrame();

                var velocityFactor = 1f / Time.deltaTime;

                var v = sampleCount % linearVelocitySamples.Length;
                var w = sampleCount % angularVelocitySamples.Length;
                sampleCount++;

                linearVelocitySamples[v] = velocityFactor * (transform.position - previousPosition);

                var deltaRotation = transform.rotation * Quaternion.Inverse(previousRotation);
                var theta = 2.0f * Mathf.Acos(Mathf.Clamp(deltaRotation.w, -1.0f, 1.0f));
                if (theta > Mathf.PI) theta -= 2.0f * Mathf.PI;

                var angularVelocity = new Vector3(deltaRotation.x, deltaRotation.y, deltaRotation.z);
                if (angularVelocity.sqrMagnitude > 0.0f)
                    angularVelocity = theta * velocityFactor * angularVelocity.normalized;

                angularVelocitySamples[w] = angularVelocity;

                previousPosition = transform.position;
                previousRotation = transform.rotation;
            }
        }
    }
}
