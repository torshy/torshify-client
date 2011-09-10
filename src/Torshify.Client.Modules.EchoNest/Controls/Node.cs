using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Torshify.Client.Modules.EchoNest.Controls
{
    public class Node<T>
        where T : IEquatable<T>
    {
        #region Fields

        private readonly T m_item;
        private readonly NodeCollection<T> m_parent;

        private ObservableCollection<Node<T>> m_children;
        private ReadOnlyObservableCollection<Node<T>> m_childrenReadOnly;

        #endregion Fields

        #region Constructors

        internal Node(T item, NodeCollection<T> parent)
        {
            Debug.Assert(item != null && parent != null);

            m_item = item;
            m_parent = parent;
        }

        #endregion Constructors

        #region Properties

        public ReadOnlyObservableCollection<Node<T>> ChildNodes
        {
            get
            {
                if (m_children == null)
                {
                    m_parent.NodeChildrenChanged += m_parent_NodeChildrenChanged;

                    m_children = new ObservableCollection<Node<T>>(m_parent.GetChildren(this.m_item));
                    m_childrenReadOnly = new ReadOnlyObservableCollection<Node<T>>(m_children);
                }
                return m_childrenReadOnly;
            }
        }

        public T Item
        {
            get { return m_item; }
        }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return "Node - '" + m_item.ToString() + "'";
        }

        private void m_parent_NodeChildrenChanged(object sender, NodeChildrenChangedArgs<T> args)
        {
            if (args.Parent.Equals(this.m_item))
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    Debug.Assert(!m_children.Contains(m_parent[args.Child]));
                    m_children.Add(m_parent[args.Child]);
                }
                else if (args.Action == NotifyCollectionChangedAction.Remove)
                {
                    Debug.Assert(m_children.Contains(m_parent[args.Child]));
                    m_children.Remove(m_parent[args.Child]);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                Debug.Assert(args.Parent != null);
            }
        }

        #endregion Methods
    }
}