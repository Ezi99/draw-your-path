using System;

namespace OctoXR.Collections
{
    [Serializable]
    public class HandBoneKeyedCollection<T> : HandBoneKeyedReadOnlyCollection<T>, IHandBoneKeyedCollection<T>
    {
        T IHandBoneKeyedCollection<T>.this[HandBoneId boneId] 
        { 
            get => items[(int)boneId];
            set => items[(int)boneId] = value; 
        }

        public new ref T this[HandBoneId boneId] => ref items[(int)boneId];
        public new ref T this[int index] => ref items[index];

        public HandBoneKeyedCollection() : base(new T[HandSkeletonConfiguration.BoneCount]) { }

        public HandBoneKeyedCollection(T[] items)
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
