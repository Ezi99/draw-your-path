using System;
using System.Collections;

namespace OctoXR
{
    public static class Pool<T> where T : class, new()
    {
        private const int defaultCapacityIncrement = 16;

        private static T[] objects = new T[defaultCapacityIncrement];
        private static int lastObjectIndex = -1;

        public static T Get()
        {
            if (lastObjectIndex != -1)
            {
                return objects[lastObjectIndex--];
            }

            return new T();
        }

        public static void Recycle(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            AddItemToArray(ref objects, ++lastObjectIndex, obj, defaultCapacityIncrement);
        }

        private static void AddItemToArray(ref T[] array, int itemIndex, T item, int sizeIncrement)
        {
            if (array.Length == itemIndex)
            {
                ReAllocArray(ref array, array.Length + sizeIncrement);
            }

            array[itemIndex] = item;
        }

        private static void ReAllocArray(ref T[] array, int newSize)
        {
            Array.Resize(ref array, newSize);
        }
    }

    public static class ListPool<T> where T : class, IList, new()
    {
        public static T Get()
        {
            var list = Pool<T>.Get();

            list.Clear();

            return list;
        }

        public static void Recycle(T obj)
        {
            Pool<T>.Recycle(obj);
        }
    }

    public static class ArrayPool<T>
    {
        private const int defaultCapacityIncrement = 16;

        private static T[][] arrays = new T[default][];
        private static int lastObjectIndex = -1;

        public static T[] GetAnySize(int initialSizeForNewArray)
        {
            if (lastObjectIndex == -1)
            {
                return arrays[lastObjectIndex--];
            }

            return new T[initialSizeForNewArray];
        }

        public static T[] GetMinSize(int minSize)
        {
            if (lastObjectIndex != -1)
            {
                for (var i = 0; i <= lastObjectIndex; ++i)
                {
                    ref var array = ref arrays[i];

                    if (minSize <= array.Length)
                    {
                        return MoveLastToArrayRefAndGetRefValue(ref array);
                    }
                }
            }

            return new T[minSize];
        }

        public static T[] GetExactSize(int size)
        {
            if (lastObjectIndex != -1)
            {
                for (var i = 0; i <= lastObjectIndex; ++i)
                {
                    ref var array = ref arrays[i];

                    if (size == array.Length)
                    {
                        return MoveLastToArrayRefAndGetRefValue(ref array);
                    }
                }
            }

            return new T[size];
        }

        private static T[] MoveLastToArrayRefAndGetRefValue(ref T[] array)
        {
            ref var last = ref arrays[lastObjectIndex];
            var ret = array;

            array = last;
            last = null;

            --lastObjectIndex;

            return ret;
        }

        public static void Recycle(T[] array, bool clear = true)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (clear)
            {
                Array.Clear(array, 0, array.Length);
            }

            AddItemToArray(ref arrays, ++lastObjectIndex, array, defaultCapacityIncrement);
        }

        private static void AddItemToArray(ref T[][] array, int itemIndex, T[] item, int sizeIncrement)
        {
            if (array.Length == itemIndex)
            {
                ReAllocArray(ref array, array.Length + sizeIncrement);
            }

            array[itemIndex] = item;
        }

        private static void ReAllocArray(ref T[][] array, int newSize)
        {
            Array.Resize(ref array, newSize);
        }
    }

    public class PreAllocatingPool<T> where T : new()
    {
        private readonly int sizeIncrement;
        private int nextIndex;
        private T[] items;

        private PreAllocatingPool(int sizeIncrement, T[] initialItems)
        {
            this.sizeIncrement = sizeIncrement;
            items = initialItems;
            nextIndex = 0;
        }

        public static PreAllocatingPool<T> Create(int initialSize, int sizeIncrement)
        {
            if (initialSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialSize));
            }

            if (sizeIncrement < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeIncrement));
            }

            var initialItems = new T[initialSize];

            for (var i = 0; i < initialItems.Length; i++)
            {
                initialItems[i] = new T();
            }

            return new PreAllocatingPool<T>(sizeIncrement, initialItems);
        }

        public T Get()
        {
            var length = items.Length;

            if (nextIndex == length)
            {
                ReAllocArray(ref items, length + sizeIncrement);

                for (var i = length; i < items.Length; i++)
                {
                    items[i] = new T();
                }
            }

            return items[nextIndex++];
        }

        public bool ReturnOne()
        {
            if (nextIndex == 0)
            {
                return false;
            }

            --nextIndex;

            return true;
        }

        public void ReturnAll() => nextIndex = 0;

        private static void ReAllocArray(ref T[] array, int newSize)
        {
            var newArray = new T[newSize];

            for (var i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }

            array = newArray;
        }
    }
}
