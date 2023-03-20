using System;

namespace OctoXR.Collections
{
    [Serializable]
    public class HandBoneKeyedReadOnlyCollection<T> : ReadOnlyCollection<T>, IHandBoneKeyedReadOnlyCollection<T>
    {
        public T this [HandBoneId boneId] => items[(int)boneId];
        public void GetItem(HandBoneId boneId, out T item) => item = items[(int)boneId];

        public HandBoneKeyedReadOnlyCollection() : base(new T[HandSkeletonConfiguration.BoneCount]) { }

        public HandBoneKeyedReadOnlyCollection(T[] items)
        {
            if (items == null)
            {
                base.items = new T[HandSkeletonConfiguration.BoneCount];
                count = items.Length;

                throw new ArgumentNullException(nameof(items));
            }

            if (items.Length < HandSkeletonConfiguration.BoneCount)
            {
                base.items = new T[HandSkeletonConfiguration.BoneCount];
                count = items.Length;

                throw new ArgumentException(
                    $"Array length cannot be less than total number of different hand bones ({HandSkeletonConfiguration.BoneCount})",
                    nameof(items));
            }

            base.items = items;
            count = HandSkeletonConfiguration.BoneCount;
        }
    }
}
