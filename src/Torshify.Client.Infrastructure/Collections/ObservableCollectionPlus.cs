using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;

namespace Torshify.Client.Infrastructure.Collections
{
    public sealed class ActionOnDispose : IDisposable
    {
        #region Fields

        private Action m_unlockDelegate;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///     Creats a new <see cref="ActionOnDispose"/>
        ///     using the provided <see cref="Action"/>.
        /// </summary>
        /// <param name="unlockAction">
        ///     The <see cref="Action"/> to invoke when <see cref="Dispose"/> is called.
        /// </param>
        /// <exception cref="ArgumentNullException">if <paramref name="unlockAction"/> is null.</exception>
        public ActionOnDispose(Action unlockAction)
        {
            Contract.Requires(unlockAction != null);

            m_unlockDelegate = unlockAction;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        ///     Calls the provided Action if it has not been called;
        ///     otherwise, throws an <see cref="Exception"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">If <see cref="Dispose()"/> has already been called.</exception>
        public void Dispose()
        {
            Action action = Interlocked.Exchange(ref m_unlockDelegate, null);
            //Util.ThrowUnless<ObjectDisposedException>(action != null, "Dispose has already been called on this object.");
            if (action != null)
            {
                action();
            }
        }

        #endregion Methods
    }

    public class ObservableCollectionPlus<T> : ObservableCollection<T>
    {
        #region Fields

        private readonly WrappedLock m_lock;
        private readonly ReadOnlyObservableCollection<T> m_roCollection;

        private bool m_isChanged;

        #endregion Fields

        #region Constructors

        public ObservableCollectionPlus()
            : this(Enumerable.Empty<T>())
        {
        }

        public ObservableCollectionPlus(IEnumerable<T> collection)
            : base(collection)
        {
            m_roCollection = new ReadOnlyObservableCollection<T>(this);
            m_lock = new WrappedLock(BeforeMultiUpdate, FinishMultiUpdate);
        }

        #endregion Constructors

        #region Properties

        public ReadOnlyObservableCollection<T> ReadOnly
        {
            get
            {
                Contract.Ensures(Contract.Result<ReadOnlyObservableCollection<T>>() != null);
                return m_roCollection;
            }
        }

        protected bool MultiUpdateActive
        {
            get { return m_lock.IsLocked; }
        }

        #endregion Properties

        #region Methods

        /// <remarks>It's recommended that you use this method within BeginMultiUpdate</remarks>
        public void AddRange(IEnumerable<T> source)
        {
            Contract.Requires(source != null);
            AppendItems(source);
        }

        public IDisposable BeginMultiUpdate()
        {
            Contract.Ensures(Contract.Result<IDisposable>() != null);
            return m_lock.GetLock();
        }

        public void Reset(IEnumerable<T> source)
        {
            using (BeginMultiUpdate())
            {
                ClearItems();
                AppendItems(source);
            }
        }

        public void Sort(Func<T, T, int> comparer)
        {
            Contract.Requires(comparer != null);
            using (this.BeginMultiUpdate())
            {
                this.QuickSort(comparer);
            }
        }

        public void Sort(IComparer<T> comparer)
        {
            Contract.Requires(comparer != null);
            using (this.BeginMultiUpdate())
            {
                this.QuickSort(comparer);
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            Contract.Requires(comparison != null);
            using (this.BeginMultiUpdate())
            {
                this.QuickSort(comparison);
            }
        }

        protected virtual void AfterMultiUpdate()
        {
        }

        protected virtual void AppendItems(IEnumerable<T> source)
        {
            Contract.Requires(source != null);
            using (BeginMultiUpdate())
            {
                foreach (var item in source)
                {
                    InsertItem(this.Count, item);
                }
            }
        }

        protected virtual void BeforeMultiUpdate()
        {
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (m_lock.IsLocked)
            {
                m_isChanged = true;
            }
            else
            {
                base.OnCollectionChanged(e);
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (m_lock.IsLocked)
            {
                m_isChanged = true;
            }
            else
            {
                base.OnPropertyChanged(e);
            }
        }

        private void FinishMultiUpdate()
        {
            AfterMultiUpdate();
            if (m_isChanged)
            {
                RaiseReset();
                m_isChanged = false;
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(m_lock != null);
            Contract.Invariant(m_roCollection != null);
        }

        private void RaiseReset()
        {
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion Methods
    }

    public static class SortHelper
    {
        #region Methods

        public static IComparer<T> ToComparer<T>(this Func<T, T, int> compareFunction)
        {
            Contract.Requires(compareFunction != null);
            return new FuncComparer<T>(compareFunction);
        }

        public static IComparer<T> ToComparer<T>(this Comparison<T> compareFunction)
        {
            Contract.Requires(compareFunction != null);
            return new ComparisonComparer<T>(compareFunction);
        }

        public static bool QuickSort<T>(this IList<T> list, Func<T, T, int> comparer)
        {
            Contract.Requires(list != null);
            Contract.Requires(comparer != null);
            return Sort(list, 0, list.Count, comparer.ToComparer());
        }

        public static bool QuickSort<T>(this IList<T> list, IComparer<T> comparer)
        {
            Contract.Requires(list != null);
            Contract.Requires(comparer != null);
            return Sort(list, 0, list.Count, comparer);
        }

        public static bool QuickSort<T>(this IList<T> list, Comparison<T> comparison)
        {
            Contract.Requires(list != null);
            Contract.Requires(comparison != null);
            return Sort(list, 0, list.Count, comparison.ToComparer());
        }

        public static bool QuickSort<T>(this IList<T> list)
        {
            Contract.Requires(list != null);
            return Sort(list, 0, list.Count, null);
        }

        private static bool quickSort<T>(IList<T> keys, int left, int right, IComparer<T> comparer)
        {
            Contract.Requires(comparer != null);
            Contract.Requires(keys != null);
            Contract.Requires(left >= 0);
            Contract.Requires(left < keys.Count);
            Contract.Requires(right >= 0);
            Contract.Requires(right < keys.Count);

            bool change = false;
            do
            {
                int a = left;
                int b = right;
                int num3 = a + ((b - a) >> 1);
                change = swapIfGreaterWithItems(keys, comparer, a, num3) || change;
                change = swapIfGreaterWithItems(keys, comparer, a, b) || change;
                change = swapIfGreaterWithItems(keys, comparer, num3, b) || change;
                T y = keys[num3];
                do
                {
                    while (comparer.Compare(keys[a], y) < 0)
                    {
                        a++;
                    }
                    while (comparer.Compare(y, keys[b]) < 0)
                    {
                        b--;
                    }
                    if (a > b)
                    {
                        break;
                    }
                    if (a < b)
                    {
                        T local2 = keys[a];
                        keys[a] = keys[b];
                        keys[b] = local2;
                        change = true;
                    }
                    a++;
                    b--;
                }
                while (a <= b);
                if ((b - left) <= (right - a))
                {
                    if (left < b)
                    {
                        change = quickSort(keys, left, b, comparer) || change;
                    }
                    left = a;
                }
                else
                {
                    if (a < right)
                    {
                        change = quickSort(keys, a, right, comparer) || change;
                    }
                    right = b;
                }
            }
            while (left < right);

            return change;
        }

        private static bool Sort<T>(IList<T> keys, int index, int length, IComparer<T> comparer)
        {
            Contract.Requires(comparer != null);
            Contract.Requires(keys != null);

            if (length > 1)
            {
                try
                {
                    if (comparer == null)
                    {
                        comparer = Comparer<T>.Default;
                    }
                    return quickSort(keys, index, index + (length - 1), comparer);
                }
                catch (IndexOutOfRangeException ioore)
                {
                    throw new ArgumentException("BogusIComparer", ioore);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException("IComparerFailed", exception);
                }
            }
            return false;
        }

        private static bool swapIfGreaterWithItems<T>(IList<T> keys, IComparer<T> comparer, int a, int b)
        {
            Contract.Requires(comparer != null);
            Contract.Requires(keys != null);
            Contract.Requires(a >= 0);
            Contract.Requires(a < keys.Count);
            Contract.Requires(a >= 0);
            Contract.Requires(b < keys.Count);

            if ((a != b) && (comparer.Compare(keys[a], keys[b]) > 0))
            {
                T local = keys[a];
                keys[a] = keys[b];
                keys[b] = local;
                return true;
            }
            else
            {
                return false;
            }
        }

        #region impl
        private class FuncComparer<T> : IComparer<T>
        {
            public FuncComparer(Func<T, T, int> func)
            {
                Contract.Requires(func != null);
                m_func = func;
            }

            public int Compare(T x, T y)
            {
                return m_func(x, y);
            }

            private readonly Func<T, T, int> m_func;
        }

        private class ComparisonComparer<T> : IComparer<T>
        {
            public ComparisonComparer(Comparison<T> func)
            {
                Contract.Requires(func != null);
                m_func = func;
            }

            public int Compare(T x, T y)
            {
                return m_func(x, y);
            }

            private readonly Comparison<T> m_func;
        }

        private class FuncEqualityComparer<T> : IEqualityComparer<T>
        {
            public FuncEqualityComparer(Func<T, T, bool> func)
            {
                Contract.Requires(func != null);
                m_func = func;
            }
            public bool Equals(T x, T y)
            {
                return m_func(x, y);
            }

            public int GetHashCode(T obj)
            {
                return 0; // this is on purpose. Should only use function...not short-cut by hashcode compare
            }

            [ContractInvariantMethod]
            void ObjectInvariant()
            {
                Contract.Invariant(m_func != null);
            }

            private readonly Func<T, T, bool> m_func;
        }
        #endregion
        #endregion Methods
    }

    public class WrappedLock : WrappedLock<object>
    {
        #region Constructors

        public WrappedLock(Action actionOnLock, Action actionOnUnlock)
            : base(WrapMaybeNull(actionOnLock), WrapMaybeNull(actionOnUnlock))
        {
        }

        #endregion Constructors

        #region Methods

        public IDisposable GetLock()
        {
            return GetLock(null);
        }

        private static Action<object> WrapMaybeNull(Action action)
        {
            return (param) =>
            {
                if (action != null) { action(); }
            };
        }

        #endregion Methods
    }

    public class WrappedLock<T>
    {
        #region Fields

        private readonly Action<T> m_actionOnLock;
        private readonly Action<T> m_actionOnUnlock;
        private readonly Stack<Guid> m_stack = new Stack<Guid>();

        #endregion Fields

        #region Constructors

        public WrappedLock(Action<T> actionOnLock, Action<T> actionOnUnlock)
        {
            m_actionOnLock = actionOnLock ?? new Action<T>((param) => { });
            m_actionOnUnlock = actionOnUnlock ?? new Action<T>((param) => { });
        }

        #endregion Constructors

        #region Properties

        public bool IsLocked
        {
            get { return m_stack.Count > 0; }
        }

        #endregion Properties

        #region Methods

        public IDisposable GetLock(T param)
        {
            Contract.Ensures(Contract.Result<IDisposable>() != null);
            if (m_stack.Count == 0)
            {
                m_actionOnLock(param);
            }

            var g = Guid.NewGuid();
            m_stack.Push(g);
            return new ActionOnDispose(() => unlock(g, param));
        }

        [ContractInvariantMethod]
        void ObjectInvariant()
        {
            Contract.Invariant(m_stack != null);
            Contract.Invariant(m_actionOnLock != null);
            Contract.Invariant(m_actionOnUnlock != null);
        }

        private void unlock(Guid g, T param)
        {
            if (m_stack.Count > 0 && m_stack.Peek() == g)
            {
                m_stack.Pop();

                if (m_stack.Count == 0)
                {
                    m_actionOnUnlock(param);
                }
            }
            else
            {
                throw new InvalidOperationException("Unlock happened in the wrong order or at a weird time or too many times");
            }
        }

        #endregion Methods
    }
}