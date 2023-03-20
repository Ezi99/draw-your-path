using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Abstract class which runtime and preview hands inherit most of the logic from.
    /// </summary>
    public abstract class PoseableHand : MonoBehaviour
    {
        [SerializeField] protected HandType HandType;

        [SerializeField] protected List<Transform> FingerRoots;
        private List<Transform> joints = new List<Transform>();

        public List<Transform> Joints { get; protected set; } = new List<Transform>();

        protected virtual void Awake()
        {
            Joints = CollectJoints();
        }

        /// <summary>
        /// Collects all the joints from the given list of finger roots and their children.
        /// </summary>
        /// <returns></returns>
        protected List<Transform> CollectJoints()
        {
            joints = new List<Transform>();

            foreach (var root in FingerRoots)
                joints.AddRange(root.GetComponentsInChildren<Transform>());

            return joints;
        }

        /// <summary>
        /// Gets joint rotations from the given list of collected joints.
        /// </summary>
        /// <returns></returns>
        public List<Quaternion> GetJointRotations()
        {
            var rotations = new List<Quaternion>();

            foreach (var joint in Joints)
                rotations.Add(joint.localRotation);

            return rotations;
        }

        protected bool HasProperCount(List<Quaternion> rotations)
        {
            return Joints.Count == rotations.Count;
        }

        protected Quaternion InvertQuaternion(List<Quaternion> rotations, int i)
        {
            Quaternion q;
            q.x = rotations[i].x;
            q.y = -rotations[i].y;
            q.z = -rotations[i].z;
            q.w = rotations[i].w;
            return q;
        }
    }
}
