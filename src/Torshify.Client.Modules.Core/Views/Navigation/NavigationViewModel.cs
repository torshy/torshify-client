using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows.Data;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Interfaces;
using System.Linq;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public class NavigationViewModel : NotificationObject, INavigationAware, IPartImportsSatisfiedNotification
    {
        #region Fields

        private readonly CompositeCollection _navigationItems;

        private IRegion _mainRegion;
        [ImportMany]
        private IEnumerable<Lazy<INavigationItemProvider, IDictionary<string,object>>> _itemProviders = null;
        private ICollectionView _navigationItemsIcv;

        #endregion Fields

        #region Constructors

        public NavigationViewModel(IRegionManager regionManager, CompositionContainer mefContainer)
        {
            _mainRegion = regionManager.Regions[CoreRegionNames.MainMusicRegion];
            _mainRegion.NavigationService.Navigated += (s, e) =>
                                                           {
                                                               var navEntry = e.NavigationContext.NavigationService.Journal.CurrentEntry;

                                                               foreach (CollectionContainer collectionContainer in _navigationItems)
                                                               {
                                                                   foreach (INavigationItem item in collectionContainer.Collection)
                                                                   {
                                                                       if (item.IsMe(navEntry) && NavigationItems.CurrentItem != item)
                                                                       {
                                                                           NavigationItems.MoveCurrentTo(item);
                                                                           break;
                                                                       }
                                                                   }
                                                               }
                                                           };
            _navigationItems = new CompositeCollection();
            NavigateToItemCommand = new AutomaticCommand<INavigationItem>(ExecuteNavigateToItem, CanExecuteNavigateToItem);
            mefContainer.SatisfyImportsOnce(this);
        }

        #endregion Constructors

        #region Properties

        public ICollectionView NavigationItems
        {
            get
            {
                return _navigationItemsIcv;
            }
            private set
            {
                _navigationItemsIcv = value;
                RaisePropertyChanged("NavigationItems");
            }
        }

        public AutomaticCommand<INavigationItem> NavigateToItemCommand
        {
            get;
            private set;
        }

        #endregion Properties

        #region Public Methods

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnImportsSatisfied()
        {
            _navigationItems.Clear();

            var r = _itemProviders.OrderBy(kp => kp.Metadata["Order"]);
            foreach (var kp in r)
            {
                CollectionContainer collectionContainer = new CollectionContainer();
                collectionContainer.Collection = kp.Value.Items;
                _navigationItems.Add(collectionContainer);
            }

            NavigationItems = CollectionViewSource.GetDefaultView(_navigationItems);
        }

        #endregion Public Methods

        #region Private Methods

        private void ExecuteNavigateToItem(INavigationItem item)
        {
            item.NavigateTo();
        }

        private bool CanExecuteNavigateToItem(INavigationItem item)
        {
            return item != NavigationItems.CurrentItem;
        }

        #endregion Private Methods
    }
}