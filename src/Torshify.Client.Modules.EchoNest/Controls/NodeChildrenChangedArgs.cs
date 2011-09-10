using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;

namespace Torshify.Client.Modules.EchoNest.Controls
{
    public class NodeChildrenChangedArgs<T> : EventArgs
    {
        #region Constructors

        public NodeChildrenChangedArgs(T parent, T child, NotifyCollectionChangedAction action)
        {
            //Contract.Requires<ArgumentNullException>(parent != null);
            //Contract.Requires<ArgumentNullException>(child != null);

            if (!(action == NotifyCollectionChangedAction.Add || action == NotifyCollectionChangedAction.Remove))
            {
                throw new ArgumentException("Only supports Add and Remove", "action");
            }

            Parent = parent;
            Child = child;
            Action = action;
        }

        #endregion Constructors

        #region Properties

        public NotifyCollectionChangedAction Action
        {
            get; private set;
        }

        public T Child
        {
            get; private set;
        }

        public T Parent
        {
            get; private set;
        }

        #endregion Properties
    }
}