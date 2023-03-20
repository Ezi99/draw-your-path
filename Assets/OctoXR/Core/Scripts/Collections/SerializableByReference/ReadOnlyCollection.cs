using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.Collections.SerializableByReference
{
	/// <summary>
	/// Provides the base class for a generic read-only collection. Unlike the <see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/>
	/// an instance of this type is serializable in Unity editor
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection</typeparam>
	/// <typeparam name="TList">The exact type of list for which the <see cref="ReadOnlyCollection{T, TList}"/> is a read-only wrapper</typeparam>
	[Serializable]
	public class ReadOnlyCollection<T, TList> : IList<T>, IList, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>
		where TList : IList<T>
		where T : class
	{
		[SerializeReference]
		private TList items;

		protected TList Items => items;

		public ReadOnlyCollection(TList items)
		{
			if (items == null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			this.items = items;
		}

		public T this[int index] => items[index];

		T IList<T>.this[int index] { get => items[index]; set => throw new NotSupportedException(); }

		public int Count => items.Count;

		bool ICollection<T>.IsReadOnly => true;
		bool IList.IsFixedSize => false;
		bool IList.IsReadOnly => true;
		int ICollection.Count => Count;
		bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => null;

        object IList.this[int index] { get => this[index]; set => throw new NotSupportedException(); }

        void ICollection<T>.Add(T item) => throw new NotSupportedException();

		void ICollection<T>.Clear() => throw new NotSupportedException();

		public bool Contains(T item) => items.Contains(item);

		public void CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

		public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

		public int IndexOf(T item) => items.IndexOf(item);

		void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

		bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

		void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

		IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        int IList.Add(object value) => throw new NotSupportedException();

        void IList.Clear() => throw new NotSupportedException();

		bool IList.Contains(object value) => value is T item && Contains(item);

		int IList.IndexOf(object value) => value is T item ? IndexOf(item) : -1;

        void IList.Insert(int index, object value) => throw new NotSupportedException();

		void IList.Remove(object value) => throw new NotSupportedException();

		void IList.RemoveAt(int index) => throw new NotSupportedException();

		void ICollection.CopyTo(Array array, int index)
        {
			if (items is ICollection itemsAsCollection)
			{
				itemsAsCollection.CopyTo(array, index);

				return;
			}

			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			if (array.Rank != 1)
			{
				throw new ArgumentException("Multidimensional arrays are not supported", nameof(array));
			}

			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException("Arrays with non zero-based indexing are not supported", nameof(array));
			}

			if (index < 0 || array.Length < index + items.Count)
			{
				throw new ArgumentException(
					"Offset into destination array was negative or destination array was not of enough size");
			}

			try
			{
				for (var i = 0; i < items.Count; ++i)
				{
					array.SetValue(items[i], index++);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
        }
    }

	/// <summary>
	/// Provides the base class for a generic read-only collection. It is an implementation of <see cref="ReadOnlyCollection{T, TList}"/>, having
	/// a <see cref="List{T}"/> to serve as a read-only wrapper for. An instance of this type is serializable in Unity editor
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection</typeparam>
	[Serializable]
	public class ReadOnlyList<T> : ReadOnlyCollection<T, List<T>> where T : class
	{
		//public ReadOnlyList() { }

		public ReadOnlyList(List<T> items) : base(items) { }
	}

	/// <summary>
	/// Provides the base class for a generic read-only collection. Unlike the <see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/>
	/// an instance of this type is serializable in Unity editor
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection</typeparam>
	[Serializable]
	public class ReadOnlyCollection<T> : IList<T>, IList, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T> where T : class
	{
		[SerializeReference]
		protected T[] items;
		[SerializeField]
		protected int count;

		public ReadOnlyCollection() : this(new T[0]) { }

		public ReadOnlyCollection(T[] items)
		{
			if (items == null)
			{
				this.items = Array.Empty<T>();

				throw new ArgumentNullException(nameof(items));
			}

			this.items = items;
			count = items.Length;
		}

		public ReadOnlyCollection(T[] items, int count)
		{
			if (items == null)
			{
				this.items = Array.Empty<T>();

				throw new ArgumentNullException(nameof(items));
			}

			if (count < 0 || count > items.Length)
			{
				this.items = items;

				throw new ArgumentOutOfRangeException(
					nameof(count),
					"Initial number of items cannot be less than zero or greater than the length of the wrapped array of items");
			}

			this.items = items;
			this.count = count;
		}

		public T this[int index]
		{
			get
			{
				if ((uint)index >= (uint)count)
				{
					throw new ArgumentOutOfRangeException(nameof(index));
				}

				return items[index];
			}
		}

		T IList<T>.this[int index] { get => items[index]; set => throw new NotSupportedException(); }

		public int Count { get => count; }

		bool ICollection<T>.IsReadOnly { get => true; }
		bool IList.IsFixedSize => false;
		bool IList.IsReadOnly => true;
		int ICollection.Count => Count;
		bool ICollection.IsSynchronized => false;
		object ICollection.SyncRoot => null;

		object IList.this[int index] { get => this[index]; set => throw new NotSupportedException(); }

		void ICollection<T>.Add(T item) => throw new NotSupportedException();

		void ICollection<T>.Clear() => throw new NotSupportedException();

		public bool Contains(T item) => List.IndexOf(items, item, count) != -1;

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
				array[arrayIndex++] = items[i];
			}
		}

		public List.Enumerator<T> GetEnumerator() => new List.Enumerator<T>(items, count);

		IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

		public int IndexOf(T item) => List.IndexOf(items, item, count);

		void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

		bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

		void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

		IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

		int IList.Add(object value) => throw new NotSupportedException();

		void IList.Clear() => throw new NotSupportedException();

		bool IList.Contains(object value) => value is T item && Contains(item);

		int IList.IndexOf(object value) => value is T item ? IndexOf(item) : -1;

		void IList.Insert(int index, object value) => throw new NotSupportedException();

		void IList.Remove(object value) => throw new NotSupportedException();

		void IList.RemoveAt(int index) => throw new NotSupportedException();

		void ICollection.CopyTo(Array array, int index)
		{
			if (items is ICollection itemsAsCollection)
			{
				itemsAsCollection.CopyTo(array, index);

				return;
			}

			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			if (array.Rank != 1)
			{
				throw new ArgumentException("Multidimensional arrays are not supported", nameof(array));
			}

			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException("Arrays with non zero-based indexing are not supported", nameof(array));
			}

			if (index < 0 || array.Length < index + count)
			{
				throw new ArgumentException(
					"Offset into destination array was negative or destination array was not of enough size");
			}

			try
			{
				for (var i = 0; i < count; ++i)
				{
					array.SetValue(items[i], index++);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
