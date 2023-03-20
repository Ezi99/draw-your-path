using OctoXR.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctoXR
{
    [CreateAssetMenu(fileName = "HandSkeletonPose", menuName = "OctoXR/Hand Skeleton Pose")]
    public class HandSkeletonPose : ScriptableObject,
        IHandBoneKeyedReadOnlyCollection<Pose>, 
        IList<Pose>, 
        IReadOnlyList<Pose>, 
        IReadOnlyCollection<Pose>, 
        IEnumerable<Pose>
    {
        private const int boneCount = HandSkeletonConfiguration.BoneCount;

        [Serializable]
        private class HandBonePoseCollection
        {
            [HandBonePropertyDrawOptions(ReadOnly = ReadOnlyPropertyDrawOptions.NotReadOnly)]
            public Pose[] Bones = CreateInitialBonePoses();

            private static Pose[] CreateInitialBonePoses()
            {
                var poses = new Pose[boneCount];

                for (int i = 0; i < poses.Length; i++)
                {
                    poses[i] = Pose.identity;
                }

                return poses;
            }
        }

        [SerializeField]
        [PropertyDrawOptions(ReadOnly = ReadOnlyPropertyDrawOptions.ReadOnlyAlways, SkipInInspectorPropertyHierarchy = true)]
        private HandBonePoseCollection bonePoseCollection = new HandBonePoseCollection();

        public ref Pose this[int index] { get => ref bonePoseCollection.Bones[index]; }
        public ref Pose this[HandBoneId boneId] { get => ref bonePoseCollection.Bones[(int)boneId]; }

        Pose IHandBoneKeyedReadOnlyCollection<Pose>.this[HandBoneId boneId] => this[boneId];

        Pose IList<Pose>.this[int index] { get => bonePoseCollection.Bones[index]; set => bonePoseCollection.Bones[index] = value; }
        Pose IReadOnlyList<Pose>.this[int index] { get => bonePoseCollection.Bones[index]; }

        public int Count { get => boneCount; }
        bool ICollection<Pose>.IsReadOnly { get => true; }

        void ICollection<Pose>.Add(Pose item) => throw new NotSupportedException();

        void ICollection<Pose>.Clear() => throw new NotSupportedException();

        public bool Contains(Pose item) => List.IndexOf(bonePoseCollection.Bones, item, boneCount) != -1;

        public void CopyTo(Pose[] array, int arrayIndex) => Array.Copy(bonePoseCollection.Bones, 0, array, arrayIndex, boneCount);

        IEnumerator<Pose> IEnumerable<Pose>.GetEnumerator() => GetEnumerator();

        public int IndexOf(Pose item) => List.IndexOf(bonePoseCollection.Bones, item, boneCount);

        void IList<Pose>.Insert(int index, Pose item) => throw new NotSupportedException();

        bool ICollection<Pose>.Remove(Pose item) => throw new NotSupportedException();

        void IList<Pose>.RemoveAt(int index) => throw new NotSupportedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public List.Enumerator<Pose> GetEnumerator() => new List.Enumerator<Pose>(bonePoseCollection.Bones);
    }
}
