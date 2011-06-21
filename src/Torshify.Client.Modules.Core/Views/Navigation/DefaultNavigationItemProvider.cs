using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    [Export(typeof(INavigationItemProvider))]
    [ExportMetadata("Order", 0)]
    public class DefaultNavigationItemProvider : INavigationItemProvider, IPartImportsSatisfiedNotification
    {
        #region Fields

        private ObservableCollection<INavigationItem> _items;

        #endregion Fields

        #region Constructors

        public DefaultNavigationItemProvider()
        {
            RegionManager = (IRegionManager)ServiceLocator.Current.GetInstance(typeof(IRegionManager));

            _items = new ObservableCollection<INavigationItem>();
            _items.Add(new PlayQueueNavigationItem(RegionManager));
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<INavigationItem> Items
        {
            get { return _items; }
        }

        protected IRegionManager RegionManager
        {
            get;
            set;
        }

        #endregion Properties

        #region Public Methods

        public void OnImportsSatisfied()
        {
        }

        #endregion Public Methods
    }
}