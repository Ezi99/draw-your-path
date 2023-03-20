using System;

namespace OctoXR.Collections
{
    [Serializable]
    public class HandFingerKeyedReadOnlyCollection<T> : ReadOnlyCollection<T>, IHandFingerKeyedReadOnlyCollection<T>
    {
        public T this [HandFinger finger] => items[(int)finger];

        public HandFingerKeyedReadOnlyCollection() : base(new T[HandSkeletonConfiguration.FingerCount]) { }

        public HandFingerKeyedReadOnlyCollection(T[] items)
        {
            if (items == null)
            {
                base.items = new T[HandSkeletonConfiguration.FingerCount];
                count = items.Length;

                throw new ArgumentNullException(nameof(items));
            }

            if (items.Length < HandSkeletonConfiguration.FingerCount)
            {
                base.items = new T[HandSkeletonConfiguration.FingerCount];
                count = items.Length;

                throw new ArgumentException(
                    $"Array length cannot be less than total number of hand fingers ({HandSkeletonConfiguration.FingerCount})",
                    nameof(items));
            }

            base.items = items;
            count = HandSkeletonConfiguration.FingerCount;
        }
    }
}
