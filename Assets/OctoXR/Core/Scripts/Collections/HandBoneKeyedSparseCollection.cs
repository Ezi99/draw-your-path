using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.Collections
{
    [Serializable]
    public class HandBoneKeyedSparseCollection<T> : IHandBoneKeyedReadOnlyCollection<T>, IList<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>
        where T : class
    {
        [SerializeField]
        [HideInInspector]
        private T[] items = new T[HandSkeletonConfiguration.BoneCount];
        [SerializeField]
        [HideInInspector]
        private int[] nonNullItemIndices = new int[HandSkeletonConfiguration.BoneCount];
        [SerializeField]
        [HideInInspector]
        private int count;

        public int Add(HandBoneId boneId, T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var boneIndex = (int)boneId;

            if (items[boneIndex] != null)
            {
                throw new ArgumentException("Item with the same bone identity is already contained in the collection");
            }

            items[boneIndex] = item;

            int insertIndex;

            for (insertIndex = 0; insertIndex < count; ++insertIndex)
            {
                var index = nonNullItemIndices[insertIndex];

                if (index == boneIndex)
                {
                    return insertIndex;
                }

                if (boneIndex < index)
                {
                    break;
                }
            }

            List.Insert(ref nonNullItemIndices, insertIndex, boneIndex, ref count);

            return insertIndex;
        }

        void ICollection<T>.Add(T item) => throw new NotSupportedException();

        public bool Remove(T item)
        {
            for (var i = 0; i < count; i++)
            {
                ref var itemRef = ref items[nonNullItemIndices[i]];

                if (itemRef == item)
                {
                    itemRef = default;

                    List.RemoveAt(nonNullItemIndices, i, ref count);

                    return true;
                }
            }

            return false;
        }

        public bool Remove(HandBoneId boneId)
        {
            ref var itemRef = ref items[(int)boneId];

            if (itemRef != null)
            {
                itemRef = default;

                List.Remove(nonNullItemIndices, (int)boneId, ref count);

                return true;
            }

            return false;
        }

        void IList<T>.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        public T RemoveAt(int index)
        {
            if ((uint)index >= (uint)count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            ref var item = ref items[nonNullItemIndices[index]];
            var itemValue = item;

            item = null;

            List.RemoveAt(nonNullItemIndices, index, ref count);

            return itemValue;
        }

        public void Clear()
        {
            for (var i = 0; i < count; i++)
            {
                items[nonNullItemIndices[i]] = default;
            }

            count = 0;
        }

        public void RemoveRange(int startIndex)
        {
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Value cannot be negative");
            }

            var newCount = count;

            for (var i = startIndex; i < count; i++)
            {
                ref var boneId = ref nonNullItemIndices[i];

                items[boneId] = default;
                boneId = default;

                --newCount;
            }

            count = newCount;
        }

        public T this[HandBoneId boneId]
        {
            get
            {
                var index = (int)boneId;
                var item = items[index];

                //if (!bone)
                //{
                //    throw new KeyNotFoundException("Bone with the specified identity is not contained in the collection");
                //}

                return item;
            }
        }

        public bool TryGetItem(HandBoneId boneId, out T item)
        {
            var index = (int)boneId;

            item = items[index];

            if (item == null)
            {
                return false;
            }

            return true;
        }

        public T this[int index] 
        {
            get
            {
                if ((uint)index >= (uint)count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return items[nonNullItemIndices[index]];
            }
        }

        public int Count { get => count; }

        bool ICollection<T>.IsReadOnly { get => false; }

        T IList<T>.this[int index] 
        {
            get => this[index];
            set => throw new NotSupportedException();
        }

        public int IndexOf(HandBoneId boneId) => List.IndexOf(nonNullItemIndices, (int)boneId, count);

        public int IndexOf(T item)
        {
            for (var i = 0; i < count; i++)
            {
                if (items[nonNullItemIndices[i]] == item)
                {
                    return i;
                }
            }

            return -1;
        }

        public HandBoneId GetBoneIdAt(int index)
        {
            if ((uint)index >= (uint)count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return (HandBoneId)nonNullItemIndices[index];
        }

        /// <summary>
        /// Returns a value that indicates whether the collection contains at least one element keyed by a bone that is part of the specified finger
        /// </summary>
        /// <param name="finger"></param>
        /// <returns></returns>
        public bool ContainsFingerBoneItem(HandFinger finger)
        {
            var fingerIndex = (int)finger;
            var startBoneIndex = fingerIndex * HandSkeletonConfiguration.RotatingBonesPerFinger + 1; // wrist root is 0 index, so offset 1 for fingers
            var endBoneIndex = startBoneIndex + HandSkeletonConfiguration.RotatingBonesPerFinger;

            for (var i = startBoneIndex; i < endBoneIndex; i++)
            {
                if (items[i] != null)
                {
                    return true;
                }
            }

            var fingerTipBoneIndex = (int)HandBoneId.ThumbFingerTip + fingerIndex;

            return items[fingerTipBoneIndex] != null;
        }

        /// <summary>
        /// Returns all items in the collection that are identified by bones that belong to the specified finger
        /// </summary>
        /// <param name="finger">The finger to obtain the items for</param>
        /// <param name="fingerBoneItems">The collection that will contain the resulting items</param>
        public void GetFingerBoneItems(HandFinger finger, ICollection<T> fingerBoneItems)
        {
            if (fingerBoneItems == null)
            {
                throw new ArgumentNullException(nameof(fingerBoneItems));
            }

            var fingerIndex = (int)finger;

            InternalGetRotatingFingerBoneItems(fingerIndex, fingerBoneItems);

            var fingerTipBoneIndex = (int)HandBoneId.ThumbFingerTip + fingerIndex;
            var fingerTipBone = items[fingerTipBoneIndex];

            if (fingerTipBone != null)
            {
                fingerBoneItems.Add(fingerTipBone);
            }
        }

        /// <summary>
        /// Returns all items in the collection that are identified by bones that belong to the specified finger. 
        /// Only the bones that rotate in the actual fingers are considered
        /// </summary>
        /// <param name="finger">The finger to obtain the items for</param>
        /// <param name="fingerBoneItems">The collection that will contain the resulting items</param>
        public void GetRotatingFingerBoneItems(HandFinger finger, ICollection<T> fingerBoneItems)
        {
            if (fingerBoneItems == null)
            {
                throw new ArgumentNullException(nameof(fingerBoneItems));
            }

            InternalGetRotatingFingerBoneItems((int)finger, fingerBoneItems);
        }

        private void InternalGetRotatingFingerBoneItems(int fingerIndex, ICollection<T> fingerBoneItems)
        {
            fingerBoneItems.Clear();

            var startBoneIndex = fingerIndex * HandSkeletonConfiguration.RotatingBonesPerFinger + 1; // wrist root is 0 index, so offset 1 for fingers
            var endBoneIndex = startBoneIndex + HandSkeletonConfiguration.RotatingBonesPerFinger;

            for (var i = startBoneIndex; i < endBoneIndex; i++)
            {
                var item = items[i];

                if (item != null)
                {
                    fingerBoneItems.Add(item);
                }
            }
        }

        void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

        public bool Contains(HandBoneId boneId) => items[(int)boneId] != null;

        public bool Contains(T item) => IndexOf(item) != -1;

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || array.Length < arrayIndex + count)
            {
                throw new ArgumentException(
                    "Offset into destination array was negative or destination array was not of enough size");
            }

            for (var i = 0; i < count; i++)
            {
                array[arrayIndex++] = items[nonNullItemIndices[i]];
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
            private readonly HandBoneKeyedSparseCollection<T> collection;
            private int currentIndex;

            public Enumerator(HandBoneKeyedSparseCollection<T> collection)
            {
                this.collection = collection;
                currentIndex = -1;
            }

            public T Current { get => collection.items[collection.nonNullItemIndices[currentIndex]]; }
            object IEnumerator.Current { get => Current; }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                return ++currentIndex < collection.count;
            }

            void IEnumerator.Reset()
            {
                currentIndex = -1;
            }
        }
    }
}
