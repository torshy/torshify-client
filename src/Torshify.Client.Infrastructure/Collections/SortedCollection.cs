using System;
using System.Collections;
using System.Collections.Generic;

namespace Torshify.Client.Infrastructure.Collections
{
    /// <summary>
    /// Collections that holds elements in the specified order. The complexity and efficiency
    /// of the algorithm is comparable to the SortedList from .NET collections. In contrast 
    /// to the SortedList SortedCollection accepts redundant elements. If no comparer is 
    /// is specified the list will use the default comparer for given type.
    /// </summary>
    /// <author>consept</author>
    public class SortedCollection<TValue> : IList<TValue>
    {
        #region Fields

        // Fields
        private const int DefaultCapacity = 4;

        private readonly IComparer<TValue> _comparer;

        private static readonly TValue[] emptyValues;

        private int _size;
        private TValue[] _values;

        // for enumeration
        private int _version;

        #endregion Fields

        #region Constructors

        static SortedCollection()
        {
            emptyValues = new TValue[0];
        }

        // Constructors
        public SortedCollection()
        {
            this._values = emptyValues;
            this._comparer = Comparer<TValue>.Default;
        }

        public SortedCollection(IComparer<TValue> comparer)
        {
            this._values = emptyValues;
            this._comparer = comparer;
        }

        #endregion Constructors

        #region Properties

        // Properties
        public int Capacity
        {
            get { return this._values.Length; }
            set
            {
                if (this._values.Length != value)
                {
                    if (value < this._size)
                    {
                        throw new ArgumentException("Too small capacity.");
                    }
                    if (value > 0)
                    {
                        TValue[] tempValues = new TValue[value];
                        if (this._size > 0)
                        {
                            // copy only when size is greater than zero
                            Array.Copy(this._values, 0, tempValues, 0, this._size);
                        }
                        this._values = tempValues;
                    }
                    else
                    {
                        this._values = emptyValues;
                    }
                }
            }
        }

        public int Count
        {
            get { return this._size; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion Properties

        #region Indexers

        public virtual TValue this[int index]
        {
            get
            {
                if (index < 0 || index >= this._size)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return this._values[index];
            }
            set
            {
                if (index < 0 || index >= this._size)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this._values[index] = value;
                this._version++;
            }
        }

        #endregion Indexers

        #region Methods

        public void Add(TValue value)
        {
            if (value == null)
            {
                throw new ArgumentException("Value can't be null");
            }
            // check where the element should be placed
            int index = Array.BinarySearch<TValue>(_values, 0, this._size, value, this._comparer);
            if (index < 0)
            {
                // xor
                index = ~index;
            }
            Insert(index, value);
        }

        public virtual void Clear()
        {
            this._version++;
            Array.Clear(this._values, 0, this._size);
            this._size = 0;
        }

        public bool Contains(TValue value)
        {
            return this.IndexOf(value) >= 0;
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            Array.Copy(this._values, 0, array, arrayIndex, this._size);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return new SortedCollectionEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SortedCollectionEnumerator(this);
        }

        public int IndexOf(TValue value)
        {
            if (value == null)
            {
                throw new ArgumentException("Value can't be null.");
            }
            int index = Array.BinarySearch<TValue>(_values, 0, this._size, value, this._comparer);
            if (index >= 0)
            {
                return index;
            }
            return -1;
        }

        public virtual void Insert(int index, TValue value)
        {
            if (value == null)
            {
                throw new ArgumentException("Value can't be null.");
            }
            if (index < 0 || index > this._size)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (this._size == this._values.Length)
            {
                this.CheckCapacity(this._size + 1);
            }
            if (index < this._size)
            {
                Array.Copy(this._values, index, this._values, index + 1, this._size - index);
            }
            this._values[index] = value;
            this._size++;
            this._version++;
        }

        public bool Remove(TValue value)
        {
            int index = this.IndexOf(value);
            if (index < 0)
            {
                return false;
            }
            RemoveAt(index);
            return true;
        }

        public virtual void RemoveAt(int index)
        {
            if (index < 0 || index >= this._size)
            {
                throw new ArgumentOutOfRangeException();
            }
            this._size--;
            this._version++;
            Array.Copy(this._values, index + 1, this._values, index, this._size - index);
            this._values[this._size] = default(TValue);
        }

        // Methods
        private void CheckCapacity(int min)
        {
            // double the capacity
            int num = this._values.Length == 0 ? DefaultCapacity : this._values.Length * 2;
            if (min > num)
            {
                num = min;
            }
            this.Capacity = num;
        }

        #endregion Methods

        #region Nested Types

        [Serializable]
        private sealed class SortedCollectionEnumerator : IEnumerator<TValue>, IDisposable, IEnumerator
        {
            #region Fields

            // Fields
            private readonly SortedCollection<TValue> collection;

            private TValue currentValue;
            private int index;
            private int version;

            #endregion Fields

            #region Constructors

            // Methods
            internal SortedCollectionEnumerator(SortedCollection<TValue> collection)
            {
                this.collection = collection;
                this.version = collection._version;
            }

            #endregion Constructors

            #region Properties

            // Properties
            public TValue Current
            {
                get
                {
                    return this.currentValue;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if ((this.index == 0) || (this.index == this.collection.Count + 1))
                    {
                        throw new ArgumentException("Enumerator not initilized. Call MoveNext first.");
                    }
                    return this.currentValue;
                }
            }

            #endregion Properties

            #region Methods

            public void Dispose()
            {
                this.index = 0;
                this.currentValue = default(TValue);
            }

            void IEnumerator.Reset()
            {
                if (this.version != this.collection._version)
                {
                    throw new ArgumentException("Collection was changed while iterating!");
                }
                this.index = 0;
                this.currentValue = default(TValue);
            }

            public bool MoveNext()
            {
                if (this.version != this.collection._version)
                {
                    throw new ArgumentException("Collection was changed while iterating!");
                }
                if (this.index < this.collection.Count)
                {
                    this.currentValue = this.collection._values[this.index];
                    this.index++;
                    return true;
                }
                this.index = this.collection.Count + 1;
                this.currentValue = default(TValue);
                return false;
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}