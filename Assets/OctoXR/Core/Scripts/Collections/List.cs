using System;
using System.Collections;
using System.Collections.Generic;

namespace OctoXR
{
    public interface IArrayResizer
    {
        void ResizeArray<T>(ref T[] array, int newSize);
    }

    /// <summary>
    /// Provides common list operations for a single-dimensional array of elements
    /// </summary>
    public static class List
    {
        private class DefaultArrayResizerClass : IArrayResizer
        {
            public void ResizeArray<T>(ref T[] array, int newSize) => Array.Resize(ref array, newSize);
        }

        public static readonly IArrayResizer DefaultArrayResizer = new DefaultArrayResizerClass();
        public const int DefaultItemsArraySizeIncrement = 8;

        public static ref T ItemAt<T>(T[] items, int index)
        {
            return ref items[index];
        }

        public static T GetItem<T>(T[] items, int index)
        {
            return ItemAt(items, index);
        }

        public static void SetItem<T>(T[] items, int index, T value)
        {
            ItemAt(items, index) = value;
        }

        public static void SetItem<T>(T[] items, int index, ref T value)
        {
            ItemAt(items, index) = value;
        }

        #region IndexOf

        public static int IndexOf<T>(
            T[] items,
            in T item,
            int itemsEndIndex)
        {
            return IndexOf(items, in item, itemsEndIndex, EqualityComparer<T>.Default, 0);
        }

        public static int IndexOf<T>(
            T[] items,
            in T item,
            IEqualityComparer<T> equalityComparer)
        {
            return IndexOf<T, IEqualityComparer<T>>(items, in item, items.Length, equalityComparer, 0);
        }

        public static int IndexOf<T>(
            T[] items,
            in T item,
            int itemsEndIndex,
            IEqualityComparer<T> equalityComparer)
        {
            return IndexOf<T, IEqualityComparer<T>>(items, in item, itemsEndIndex, equalityComparer, 0);
        }

        public static int IndexOf<T>(
            T[] items,
            in T item,
            int itemsEndIndex,
            int itemsStartIndex)
        {
            return IndexOf(items, in item, itemsEndIndex, EqualityComparer<T>.Default, itemsStartIndex);
        }

        public static int IndexOf<T, TEqualityComparer>(
            T[] items,
            in T item,
            TEqualityComparer equalityComparer)
            where TEqualityComparer : IEqualityComparer<T>
        {
            return IndexOf<T, TEqualityComparer>(items, in item, items.Length, equalityComparer, 0);
        }

        public static int IndexOf<T, TEqualityComparer>(
            T[] items,
            in T item,
            int itemsEndIndex,
            TEqualityComparer equalityComparer)
            where TEqualityComparer : IEqualityComparer<T>
        {
            return IndexOf(items, in item, itemsEndIndex, equalityComparer, 0);
        }

        public static int IndexOf<T>(
            T[] items,
            in T item,
            int itemsEndIndex,
            IEqualityComparer<T> equalityComparer,
            int itemsStartIndex)
        {
            return IndexOf<T, IEqualityComparer<T>>(items, in item, itemsEndIndex, equalityComparer, itemsStartIndex);
        }

        public static int IndexOf<T, TEqualityComparer>(
            T[] items,
            in T item,
            int itemsEndIndex,
            TEqualityComparer equalityComparer,
            int itemsStartIndex)
            where TEqualityComparer : IEqualityComparer<T>
        {
            for (var i = itemsStartIndex; i < itemsEndIndex; ++i)
            {
                if (equalityComparer.Equals(item, items[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region Add

        public static void Add<T>(
            ref T[] items,
            in T item,
            ref int itemCount)
        {
            Add<T, IArrayResizer>(ref items, in item, ref itemCount, DefaultItemsArraySizeIncrement, DefaultArrayResizer);
        }

        public static void Add<T>(
            ref T[] items,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement)
        {
            Add<T, IArrayResizer>(ref items, in item, ref itemCount, itemsArraySizeIncrement, DefaultArrayResizer);
        }

        public static void Add<T>(
            ref T[] items,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer)
        {
            Add<T, IArrayResizer>(ref items, in item, ref itemCount, itemsArraySizeIncrement, arrayResizer);
        }

        public static void Add<T, TArrayResizer>(
            ref T[] items,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer)
            where TArrayResizer : IArrayResizer
        {
            var newItemCount = EnsureItemsArrayCanAccomodateOneMoreElement(ref items, itemCount, itemsArraySizeIncrement, arrayResizer);

            items[itemCount] = item;

            itemCount = newItemCount;
        }

        public static void Add<T>(
            ref T[] items,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsEndIndex)
        {
            Add<T, IArrayResizer>(ref items, in item, ref itemCount, itemsArraySizeIncrement, DefaultArrayResizer, itemsEndIndex);
        }

        public static void Add<T>(
            ref T[] items,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsEndIndex)
        {
            Add<T, IArrayResizer>(ref items, in item, ref itemCount, itemsArraySizeIncrement, arrayResizer, itemsEndIndex);
        }

        public static void Add<T, TArrayResizer>(
            ref T[] items,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsEndIndex)
            where TArrayResizer : IArrayResizer
        {
            EnsureItemsArrayCanAccomodateOneMoreElement(ref items, itemsEndIndex, itemsArraySizeIncrement, arrayResizer);

            items[itemsEndIndex] = item;

            ++itemCount;
        }

        #endregion

        #region AddRange

        #region From array

        public static void AddRange<T>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount)
        {
            AddRange<T, IArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAdd.Length);
        }

        public static void AddRange<T>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsToAddCount)
        {
            AddRange<T, IArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            AddRange<T, IArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            AddRange<T, IArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            AddRange<T, IArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer)
        {
            AddRange<T, IArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Length);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer)
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Length);
        }

        public static void AddRange<T>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
        {
            AddRange<T, IArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            AddRange<T, IArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer)
        {
            AddRange<T, IArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Length);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer)
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Length);
        }

        public static void AddRange<T>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
        {
            AddRange<T, IArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            AddRange<T, IArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            AddRange<T, IArrayResizer>(
                ref items,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            var newItemsEndIndex = itemsEndIndex + itemsToAddCount;

            EnsureItemsArrayCapacity(ref items, newItemsEndIndex, itemsArraySizeIncrement, arrayResizer);

            var itemsToAddEndIndex = itemsToAddStartIndex + itemsToAddCount;

            for (var i = itemsToAddStartIndex; i < itemsToAddEndIndex; ++i)
            {
                items[itemsEndIndex++] = itemsToAdd[i];
            }

            itemCount += itemsToAddCount;
        }

        #endregion

        #region From list

        #region List by value

        #region Non-generic

        public static void AddRange<T>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount)
        {
            AddRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsToAddCount)
        {
            AddRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            AddRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            AddRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            AddRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer)
        {
            AddRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer)
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
        {
            AddRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            AddRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer)
        {
            AddRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer)
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
        {
            AddRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            AddRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            AddRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TArrayResizer>(
            ref T[] items,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        #endregion

        #region Generic

        public static void AddRange<T, TList>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TList, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TList, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TList, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TList, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TList, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TList, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TList, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        #endregion

        #endregion

        #region List by ref

        public static void AddRange<T, TList>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TList, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TList, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TList, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TList, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TList, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            AddRange<T, TList, TArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            AddRange<T, TList, IArrayResizer>(
                ref items,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void AddRange<T, TList, TArrayResizer>(
            ref T[] items,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            var newItemsEndIndex = itemsEndIndex + itemsToAddCount;

            EnsureItemsArrayCapacity(ref items, newItemsEndIndex, itemsArraySizeIncrement, arrayResizer);

            var itemsToAddEndIndex = itemsToAddStartIndex + itemsToAddCount;

            for (var i = itemsToAddStartIndex; i < itemsToAddEndIndex; ++i)
            {
                items[itemsEndIndex++] = itemsToAdd[i];
            }

            itemCount += itemsToAddCount;
        }

        #endregion

        #endregion

        #endregion

        #region Insert

        public static void Insert<T>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount)
        {
            Insert<T, IArrayResizer>(ref items, index, in item, ref itemCount, DefaultItemsArraySizeIncrement, DefaultArrayResizer);
        }

        public static void Insert<T>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement)
        {
            Insert<T, IArrayResizer>(ref items, index, in item, ref itemCount, itemsArraySizeIncrement, DefaultArrayResizer);
        }

        public static void Insert<T>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer)
        {
            Insert<T, IArrayResizer>(ref items, index, in item, ref itemCount, itemsArraySizeIncrement, arrayResizer);
        }

        public static void Insert<T, TArrayResizer>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer)
            where TArrayResizer : IArrayResizer
        {
            var newItemCount = EnsureItemsArrayCanAccomodateOneMoreElement(ref items, itemCount, itemsArraySizeIncrement, arrayResizer);

            InsertAndMoveElements(items, index, in item, itemCount);

            itemCount = newItemCount;
        }

        public static void Insert<T>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsEndIndex)
        {
            Insert<T, IArrayResizer>(ref items, index, in item, ref itemCount, itemsArraySizeIncrement, DefaultArrayResizer, itemsEndIndex);
        }

        public static void Insert<T>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsEndIndex)
        {
            Insert<T, IArrayResizer>(ref items, index, in item, ref itemCount, itemsArraySizeIncrement, arrayResizer, itemsEndIndex);
        }

        public static void Insert<T, TArrayResizer>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsEndIndex)
            where TArrayResizer : IArrayResizer
        {
            EnsureItemsArrayCanAccomodateOneMoreElement(ref items, itemsEndIndex, itemsArraySizeIncrement, arrayResizer);
            InsertAndMoveElements(items, index, in item, itemsEndIndex);

            ++itemCount;
        }

        private static void InsertAndMoveElements<T>(T[] items, int index, in T item, int itemsEndIndex)
        {
            ref var current = ref items[index];
            var currentValue = current;

            current = item;

            for (var i = index; i < itemsEndIndex; ++i)
            {
                ref var itemNext = ref items[i + 1];
                var itemNextValue = itemNext;

                itemNext = currentValue;
                currentValue = itemNextValue;
            }
        }

        #endregion

        #region InsertMoveCurrentToEnd

        public static void InsertMoveCurrentToEnd<T>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount)
        {
            InsertMoveCurrentToEnd<T, IArrayResizer>(ref items, index, in item, ref itemCount, DefaultItemsArraySizeIncrement, DefaultArrayResizer);
        }

        public static void InsertMoveCurrentToEnd<T>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement)
        {
            InsertMoveCurrentToEnd<T, IArrayResizer>(ref items, index, in item, ref itemCount, itemsArraySizeIncrement, DefaultArrayResizer);
        }

        public static void InsertMoveCurrentToEnd<T>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer)
        {
            InsertMoveCurrentToEnd<T, IArrayResizer>(ref items, index, in item, ref itemCount, itemsArraySizeIncrement, arrayResizer);
        }

        public static void InsertMoveCurrentToEnd<T, TArrayResizer>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer)
            where TArrayResizer : IArrayResizer
        {
            var newItemCount = EnsureItemsArrayCanAccomodateOneMoreElement(ref items, itemCount, itemsArraySizeIncrement, arrayResizer);

            InsertMoveCurrentToEnd(items, index, in item, itemCount);

            itemCount = newItemCount;
        }

        public static void InsertMoveCurrentToEnd<T>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsEndIndex)
        {
            InsertMoveCurrentToEnd<T, IArrayResizer>(ref items, index, in item, ref itemCount, itemsArraySizeIncrement, DefaultArrayResizer, itemsEndIndex);
        }

        public static void InsertMoveCurrentToEnd<T>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsEndIndex)
        {
            InsertMoveCurrentToEnd<T, IArrayResizer>(ref items, index, in item, ref itemCount, itemsArraySizeIncrement, arrayResizer, itemsEndIndex);
        }

        public static void InsertMoveCurrentToEnd<T, TArrayResizer>(
            ref T[] items,
            int index,
            in T item,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsEndIndex)
            where TArrayResizer : IArrayResizer
        {
            EnsureItemsArrayCanAccomodateOneMoreElement(ref items, itemsEndIndex, itemsArraySizeIncrement, arrayResizer);
            InsertMoveCurrentToEnd(items, index, in item, itemsEndIndex);

            ++itemCount;
        }

        public static void InsertMoveCurrentToEnd<T>(T[] items, int index, in T item, int itemsEndIndex)
        {
            ref var itemAtIndex = ref items[index];
            var itemAtIndexValue = itemAtIndex;

            itemAtIndex = item;
            items[itemsEndIndex] = itemAtIndexValue;
        }

        #endregion

        #region InsertRange

        #region From array

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount)
        {
            InsertRange<T, IArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAdd.Length);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsToAddCount)
        {
            InsertRange<T, IArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            InsertRange<T, IArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            InsertRange<T, IArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            InsertRange<T, IArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer)
        {
            InsertRange<T, IArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Length);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer)
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Length);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
        {
            InsertRange<T, IArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            InsertRange<T, IArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer)
        {
            InsertRange<T, IArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Length);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer)
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Length);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
        {
            InsertRange<T, IArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            InsertRange<T, IArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            InsertRange<T, IArrayResizer>(
                ref items,
                index,
                itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            T[] itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            var newItemsEndIndex = itemsEndIndex + itemsToAddCount;

            EnsureItemsArrayCapacity(ref items, newItemsEndIndex, itemsArraySizeIncrement, arrayResizer);

            var itemsToAddEndIndex = itemsToAddStartIndex + itemsToAddCount;

            for (var i = itemsToAddStartIndex; i < itemsToAddEndIndex; ++i)
            {
                ref var current = ref items[index];
                var currentValue = current;

                current = itemsToAdd[i];

                for (var j = index++; j < itemCount; j += itemsToAddCount)
                {
                    ref var itemNext = ref items[j + itemsToAddCount];
                    var itemNextValue = itemNext;

                    itemNext = currentValue;
                    currentValue = itemNextValue;
                }
            }

            itemCount += itemsToAddCount;
        }

        #endregion

        #region From list

        #region List by value

        #region Non-generic

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount)
        {
            InsertRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsToAddCount)
        {
            InsertRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            InsertRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            InsertRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            InsertRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer)
        {
            InsertRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer)
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
        {
            InsertRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            InsertRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer)
        {
            InsertRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer)
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
        {
            InsertRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            InsertRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
        {
            InsertRange<T, IReadOnlyList<T>, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TArrayResizer>(
            ref T[] items,
            int index,
            IReadOnlyList<T> itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, IReadOnlyList<T>, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        #endregion

        #region Generic

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TList, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TList, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TList, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TList, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TList, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TList, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TList, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        #endregion

        #endregion

        #region List by ref

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                DefaultArrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TList, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TList, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TList, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                DefaultItemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TList, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAdd.Count);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TList, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                0,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            InsertRange<T, TList, TArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemCount,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            IArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
        {
            InsertRange<T, TList, IArrayResizer>(
                ref items,
                index,
                ref itemsToAdd,
                ref itemCount,
                itemsArraySizeIncrement,
                arrayResizer,
                itemsEndIndex,
                itemsToAddStartIndex,
                itemsToAddCount);
        }

        public static void InsertRange<T, TList, TArrayResizer>(
            ref T[] items,
            int index,
            ref TList itemsToAdd,
            ref int itemCount,
            int itemsArraySizeIncrement,
            TArrayResizer arrayResizer,
            int itemsEndIndex,
            int itemsToAddStartIndex,
            int itemsToAddCount)
            where TList : IReadOnlyList<T>
            where TArrayResizer : IArrayResizer
        {
            var newItemsEndIndex = itemsEndIndex + itemsToAddCount;

            EnsureItemsArrayCapacity(ref items, newItemsEndIndex, itemsArraySizeIncrement, arrayResizer);

            var itemsToAddIndexEnd = itemsToAddStartIndex + itemsToAddCount;

            for (var i = itemsToAddStartIndex; i < itemsToAddIndexEnd; ++i)
            {
                ref var current = ref items[index];
                var currentValue = current;

                current = itemsToAdd[i];

                for (var j = index++; j < itemCount; j += itemsToAddCount)
                {
                    ref var itemNext = ref items[j + itemsToAddCount];
                    var itemNextValue = itemNext;

                    itemNext = currentValue;
                    currentValue = itemNextValue;
                }
            }

            itemCount += itemsToAddCount;
        }

        #endregion

        #endregion

        #endregion

        #region Remove

        public static bool Remove<T>(
            T[] items,
            in T item,
            ref int itemCount)
        {
            return Remove<T, EqualityComparer<T>>(items, in item, ref itemCount, EqualityComparer<T>.Default);
        }

        public static bool Remove<T>(
            T[] items,
            in T item,
            ref int itemCount,
            int itemsEndIndex)
        {
            return Remove<T, EqualityComparer<T>>(items, in item, ref itemCount, itemsEndIndex, EqualityComparer<T>.Default, 0);
        }

        public static bool Remove<T>(
            T[] items,
            in T item,
            ref int itemCount,
            int itemsEndIndex,
            int itemsStartIndex)
        {
            return Remove<T, EqualityComparer<T>>(items, in item, ref itemCount, itemsEndIndex, EqualityComparer<T>.Default, itemsStartIndex);
        }

        public static bool Remove<T>(
            T[] items,
            in T item,
            ref int itemCount,
            IEqualityComparer<T> equalityComparer)
        {
            return Remove<T, IEqualityComparer<T>>(items, in item, ref itemCount, equalityComparer);
        }

        public static bool Remove<T, TEqualityComparer>(
            T[] items,
            in T item,
            ref int itemCount,
            TEqualityComparer equalityComparer)
            where TEqualityComparer : IEqualityComparer<T>
        {
            var index = IndexOf(items, in item, itemCount, equalityComparer, 0);

            if (index != -1)
            {
                RemoveAt(items, index, ref itemCount);

                return true;
            }

            return false;
        }

        public static bool Remove<T>(
            T[] items,
            in T item,
            ref int itemCount,
            int itemsEndIndex,
            IEqualityComparer<T> equalityComparer)
        {
            return Remove<T, IEqualityComparer<T>>(items, in item, ref itemCount, itemsEndIndex, equalityComparer, 0);
        }

        public static bool Remove<T, TEqualityComparer>(
            T[] items,
            in T item,
            ref int itemCount,
            int itemsEndIndex,
            TEqualityComparer equalityComparer)
            where TEqualityComparer : IEqualityComparer<T>
        {
            return Remove<T, TEqualityComparer>(items, in item, ref itemCount, itemsEndIndex, equalityComparer, 0);
        }

        public static bool Remove<T>(
            T[] items,
            in T item,
            ref int itemCount,
            int itemsEndIndex,
            IEqualityComparer<T> equalityComparer,
            int itemsStartIndex)
        {
            return Remove<T, IEqualityComparer<T>>(items, in item, ref itemCount, itemsEndIndex, equalityComparer, itemsStartIndex);
        }

        public static bool Remove<T, TEqualityComparer>(
            T[] items,
            in T item,
            ref int itemCount,
            int itemsEndIndex,
            TEqualityComparer equalityComparer,
            int itemsStartIndex)
            where TEqualityComparer : IEqualityComparer<T>
        {
            var index = IndexOf(items, in item, itemsEndIndex, equalityComparer, itemsStartIndex);

            if (index != -1)
            {
                RemoveAt(items, index, ref itemCount, itemsEndIndex);

                return true;
            }

            return false;
        }

        #endregion

        #region RemoveMoveLastToRemoved

        public static bool RemoveMoveLastToRemoved<T>(
            T[] items,
            in T item,
            ref int itemCount)
        {
            return RemoveMoveLastToRemoved<T, EqualityComparer<T>>(items, in item, ref itemCount, EqualityComparer<T>.Default);
        }

        public static bool RemoveMoveLastToRemoved<T>(
            T[] items,
            in T item,
            ref int itemCount,
            int itemsEndIndex)
        {
            return RemoveMoveLastToRemoved<T, EqualityComparer<T>>(items, in item, ref itemCount, itemsEndIndex, EqualityComparer<T>.Default, 0);
        }

        public static bool RemoveMoveLastToRemoved<T>(
            T[] items,
            in T item,
            ref int itemCount,
            int itemsEndIndex,
            int itemsStartIndex)
        {
            return RemoveMoveLastToRemoved<T, EqualityComparer<T>>(items, in item, ref itemCount, itemsEndIndex, EqualityComparer<T>.Default, itemsStartIndex);
        }

        public static bool RemoveMoveLastToRemoved<T>(
            T[] items,
            in T item,
            ref int itemCount,
            IEqualityComparer<T> equalityComparer)
        {
            return RemoveMoveLastToRemoved<T, IEqualityComparer<T>>(items, in item, ref itemCount, equalityComparer);
        }

        public static bool RemoveMoveLastToRemoved<T, TEqualityComparer>(
            T[] items,
            in T item,
            ref int itemCount,
            TEqualityComparer equalityComparer)
            where TEqualityComparer : IEqualityComparer<T>
        {
            var index = IndexOf(items, in item, itemCount, equalityComparer, 0);

            if (index != -1)
            {
                RemoveAtMoveLastToRemoved(items, index, ref itemCount);

                return true;
            }

            return false;
        }

        public static bool RemoveMoveLastToRemoved<T>(
            T[] items,
            in T item,
            ref int itemCount,
            int itemsEndIndex,
            IEqualityComparer<T> equalityComparer)
        {
            return RemoveMoveLastToRemoved<T, IEqualityComparer<T>>(items, in item, ref itemCount, itemsEndIndex, equalityComparer, 0);
        }

        public static bool RemoveMoveLastToRemoved<T, TEqualityComparer>(
            T[] items,
            in T item,
            ref int itemCount,
            int itemsEndIndex,
            TEqualityComparer equalityComparer)
            where TEqualityComparer : IEqualityComparer<T>
        {
            return RemoveMoveLastToRemoved<T, TEqualityComparer>(items, in item, ref itemCount, itemsEndIndex, equalityComparer, 0);
        }

        public static bool RemoveMoveLastToRemoved<T>(
            T[] items,
            in T item,
            ref int itemCount,
            int itemsEndIndex,
            IEqualityComparer<T> equalityComparer,
            int itemsStartIndex)
        {
            return RemoveMoveLastToRemoved<T, IEqualityComparer<T>>(items, in item, ref itemCount, itemsEndIndex, equalityComparer, itemsStartIndex);
        }

        public static bool RemoveMoveLastToRemoved<T, TEqualityComparer>(
            T[] items,
            in T item,
            ref int itemCount,
            int itemsEndIndex,
            TEqualityComparer equalityComparer,
            int itemsStartIndex)
            where TEqualityComparer : IEqualityComparer<T>
        {
            var index = IndexOf(items, in item, itemsEndIndex, equalityComparer, itemsStartIndex);

            if (index != -1)
            {
                RemoveAtMoveLastToRemoved(items, index, ref itemCount, itemsEndIndex);

                return true;
            }

            return false;
        }

        #endregion

        #region RemoveRange

        public static void RemoveRange<T>(T[] items, int index, int removeCount, ref int itemCount)
        {
            RemoveRange(items, index, removeCount, ref itemCount, itemCount);
        }

        public static void RemoveRange<T>(T[] items, int index, int removeCount, ref int itemCount, int itemsEndIndex)
        {
            var indexRemoveEnd = index + removeCount;

            for (var i = indexRemoveEnd; i < itemsEndIndex; ++i)
            {
                items[index++] = items[i];
            }

            for (var i = index; i < itemsEndIndex; ++i)
            {
                items[i] = default;
            }

            itemCount -= removeCount;
        }

        #endregion

        #region Clear

        public static void Clear<T>(T[] items, ref int itemCount)
        {
            Clear(items, ref itemCount, 0);
        }

        public static void Clear<T>(T[] items, ref int itemCount, int itemsStartIndex)
        {
            var indexEnd = itemsStartIndex + itemCount;

            for (var i = itemsStartIndex; i < indexEnd; ++i)
            {
                items[i] = default;
            }

            itemCount = 0;
        }

        #endregion

        #region RemoveAt

        public static void RemoveAt<T>(T[] items, int index, ref int itemCount)
        {
            ref var current = ref items[index];

            --itemCount;

            for (var i = index; i < itemCount; ++i)
            {
                ref var item = ref items[i + 1];

                current = item;
                current = ref item;
            }

            current = default;
        }

        public static void RemoveAt<T>(T[] items, int index, ref int itemCount, int itemsEndIndex)
        {
            ref var current = ref items[index];

            --itemsEndIndex;

            for (var i = index; i < itemsEndIndex; ++i)
            {
                ref var item = ref items[i + 1];

                current = item;
                current = ref item;
            }

            current = default;

            --itemCount;
        }

        #endregion

        #region RemoveAtMoveLastToRemoved

        public static void RemoveAtMoveLastToRemoved<T>(T[] items, int index, ref int itemCount)
        {
            ref var item = ref items[index];
            ref var itemLast = ref items[--itemCount];

            item = itemLast;
            itemLast = default;
        }

        public static void RemoveAtMoveLastToRemoved<T>(T[] items, int index, ref int itemCount, int itemsEndIndex)
        {
            ref var item = ref items[index];
            ref var itemLast = ref items[itemsEndIndex - 1];

            item = itemLast;
            itemLast = default;

            --itemCount;
        }

        #endregion

        #region CopyTo

        public static void CopyTo<T>(T[] sourceArray, T[] destinationArray)
        {
            CopyTo(sourceArray, destinationArray, 0, sourceArray.Length, 0);
        }

        public static void CopyTo<T>(T[] sourceArray, T[] destinationArray, int srcEndIndex)
        {
            CopyTo(sourceArray, destinationArray, 0, srcEndIndex, 0);
        }

        public static void CopyTo<T>(T[] sourceArray, T[] destinationArray, int srcEndIndex, int dstStartIndex)
        {
            CopyTo(sourceArray, destinationArray, 0, srcEndIndex, dstStartIndex);
        }

        public static void CopyTo<T>(T[] sourceArray, T[] destinationArray, int srcStartIndex, int srcEndIndex, int dstStartIndex)
        {
            for (var i = srcStartIndex; i < srcEndIndex; ++i)
            {
                destinationArray[dstStartIndex++] = sourceArray[i];
            }
        }

        #endregion

        #region Enumerator

        public struct Enumerator<T> : IEnumerator<T>
        {
            private readonly T[] items;
            private int currentIndex;
            private readonly int itemsStartIndex;
            private readonly int itemsEndIndex;

            public Enumerator(T[] items)
            {
                this.items = items;
                currentIndex = -1;
                itemsStartIndex = 0;
                itemsEndIndex = items.Length;
            }

            public Enumerator(T[] items, int itemCount)
            {
                this.items = items;
                currentIndex = -1;
                itemsStartIndex = 0;
                itemsEndIndex = itemCount;
            }

            public Enumerator(T[] items, int itemsStartIndex, int itemsEndIndex)
            {
                this.items = items;
                currentIndex = itemsStartIndex - 1;
                this.itemsStartIndex = itemsStartIndex;
                this.itemsEndIndex = itemsEndIndex;
            }

            public T Current { get => items[currentIndex]; }
            object IEnumerator.Current { get => Current; }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                return ++currentIndex < itemsEndIndex;
            }

            void IEnumerator.Reset()
            {
                currentIndex = itemsStartIndex - 1;
            }
        }

        #endregion

        private static int EnsureItemsArrayCanAccomodateOneMoreElement<T, TArrayResizer>(
            ref T[] items,
            int currentItemsEndIndex,
            int itemsCapacityIncrement,
            TArrayResizer arrayResizer)
            where TArrayResizer : IArrayResizer
        {
            var arrayLength = items.Length;

            if (++currentItemsEndIndex > arrayLength)
            {
                arrayResizer.ResizeArray(ref items, arrayLength + itemsCapacityIncrement);
            }

            return currentItemsEndIndex;
        }

        private static void EnsureItemsArrayCapacity<T, TArrayResizer>(
            ref T[] items,
            int newItemsEndIndex,
            int itemsCapacityIncrement,
            TArrayResizer arrayResizer)
            where TArrayResizer : IArrayResizer
        {
            var arrayLength = items.Length;

            if (newItemsEndIndex > arrayLength)
            {
                var quotient = Math.DivRem(newItemsEndIndex - arrayLength, itemsCapacityIncrement, out var remainder);
                var incrementScale = quotient + Math.Min(1, remainder);

                arrayResizer.ResizeArray(ref items, arrayLength + incrementScale * itemsCapacityIncrement);
            }
        }
    }
}
