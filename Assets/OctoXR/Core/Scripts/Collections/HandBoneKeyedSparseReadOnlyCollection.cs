using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.Collections
{
    /// <summary>
    /// A read-only wrapper around a <see cref="HandBoneKeyedSparseCollection{T}"/>. Serializable in Unity editor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class HandBoneKeyedSparseReadOnlyCollection<T> : IHandBoneKeyedReadOnlyCollection<T>, IList<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>
        where T : class
    {
        [SerializeField]
        [HideInInspector]
        private HandBoneKeyedSparseCollection<T> items;
        protected HandBoneKeyedSparseCollection<T> Items => items;

        public HandBoneKeyedSparseReadOnlyCollection()
        {
            items = new HandBoneKeyedSparseCollection<T>();
        }

        public HandBoneKeyedSparseReadOnlyCollection(HandBoneKeyedSparseCollection<T> items)
        {
            if (items == null)
            {
                this.items = new HandBoneKeyedSparseCollection<T>();

                throw new ArgumentNullException(nameof(items));
            }

            this.items = items;
        }

        public T this[HandBoneId boneId] => items[boneId];
        public T this[int index] => items[index];
        public bool TryGetItem(HandBoneId boneId, out T item) => items.TryGetItem(boneId, out item);
        public int Count => items.Count;
        bool ICollection<T>.IsReadOnly { get => true; }

        T IList<T>.this[int index] 
        {
            get => items[index];
            set => throw new NotSupportedException();
        }

        public int IndexOf(HandBoneId boneId) => items.IndexOf(boneId);
        public int IndexOf(T item) => items.IndexOf(item);
        public HandBoneId GetBoneIdAt(int index) => items.GetBoneIdAt(index);

        /// <summary>
        /// Returns a value that indicates whether the collection contains at least one element keyed by a bone that is part of the specified finger
        /// </summary>
        /// <param name="finger"></param>
        /// <returns></returns>
        public bool ContainsFingerBoneItem(HandFinger finger) => items.ContainsFingerBoneItem(finger);

        /// <summary>
        /// Returns all items in the collection that are identified by bones that belong to the specified finger
        /// </summary>
        /// <param name="finger">The finger to obtain the items for</param>
        /// <param name="fingerBoneItems">The collection that will contain the resulting items</param>
        public void GetFingerBoneItems(HandFinger finger, ICollection<T> fingerBoneItems) => items.GetFingerBoneItems(finger, fingerBoneItems);

        /// <summary>
        /// Returns all items in the collection that are identified by bones that belong to the specified finger. 
        /// Only the bones that rotate in the actual fingers are considered
        /// </summary>
        /// <param name="finger">The finger to obtain the items for</param>
        /// <param name="fingerBoneItems">The collection that will contain the resulting items</param>
        public void GetRotatingFingerBoneItems(HandFinger finger, ICollection<T> fingerBoneItems) => items.GetRotatingFingerBoneItems(finger, fingerBoneItems);

        void IList<T>.Insert(int index, T item) => throw new NotSupportedException();
        void IList<T>.RemoveAt(int index) => throw new NotSupportedException();
        void ICollection<T>.Add(T item) => throw new NotSupportedException();
        bool ICollection<T>.Remove(T item) => throw new NotSupportedException();
        void ICollection<T>.Clear() => throw new NotSupportedException();

        public bool Contains(HandBoneId boneId) => items.Contains(boneId);
        public bool Contains(T item) => items.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        public HandBoneKeyedSparseCollection<T>.Enumerator GetEnumerator() => items.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    }
}
