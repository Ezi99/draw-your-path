using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.HandPoseDetection
{
    public class HandPoseShape : MonoBehaviour, IHandPoseComponent
    {
        [SerializeField] private List<HandShape> shapes;
        private HandSkeleton handSkeleton;

        public bool Detect()
        {
            var isNullOrEmpty = shapes == null || shapes.Count == 0;

            if (isNullOrEmpty)
            {
                var log = LogUtility.FormatLogMessageFromComponent(this, "Hand pose shape not defined. Shapes array is empty!");
                Debug.LogError(log);

                return false;
            }

            for (var i = 0; i < shapes.Count; i++)
            {
                if (!shapes[i].IsDetected(handSkeleton)) return false;
            }

            return true;
        }

        public void InjectHandSkeleton(HandSkeleton handSkeleton)
        {
            this.handSkeleton = handSkeleton;
        }

        public void AddHandShape(HandShape shape)
        {
            shapes ??= new List<HandShape>();

            shapes.Add(shape);
        }
    }
}
