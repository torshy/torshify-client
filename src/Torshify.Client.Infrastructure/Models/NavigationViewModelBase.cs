using System;
using System.Collections.ObjectModel;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Client.Infrastructure.Models
{
    public abstract class NavigationViewModelBase<T> : NotificationObject
        where T : NavigationItem
    {
        #region Fields

        private readonly IRegionManager _regionManager;

        private bool _disregardNavigation;
        private ObservableCollection<T> _navigationItems;

        #endregion Fields

        #region Constructors

        protected NavigationViewModelBase(IRegionManager regionManager)
        {
            _navigationItems = new ObservableCollection<T>();
            _regionManager = regionManager;

            if (_regionManager.Regions.ContainsRegionWithName(CoreRegionNames.MainMusicRegion))
            {
                _regionManager.Regions[CoreRegionNames.MainMusicRegion].NavigationService.Navigated +=
                    OnTargetRegionNavigated;
            }
            else
            {
                _regionManager.Regions.CollectionChanged += OnRegionsCollectionChanged;
            }
        }

        #endregion Constructors

        #region Properties

        protected ObservableCollection<T> NavigationItems
        {
            get
            {
                return _navigationItems;
            }
        }

        #endregion Properties

        #region Methods

        public virtual void SelectedItemChanged(T oldItem, T newItem)
        {
            if (newItem == null && !CanNavigateToItem(newItem))
            {
                return;
            }

            if (_disregardNavigation)
                return;

            _regionManager
                .RequestNavigate(
                    newItem.RegionName,
                    newItem.NavigationUrl,
                    GetNavigationContextTag(newItem));
        }

        protected abstract bool CanNavigateToItem(T item);

        protected virtual object GetNavigationContextTag(T item)
        {
            return item;
        }

        protected virtual bool IsFromThisItem(IRegionNavigationJournalEntry entry, T item)
        {
            var parts = entry.Uri.OriginalString.Split('?');
            return parts[0] == item.NavigationUrl.OriginalString;
        }

        void OnRegionsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_regionManager.Regions.ContainsRegionWithName(CoreRegionNames.MainMusicRegion))
            {
                _regionManager.Regions[CoreRegionNames.MainMusicRegion].NavigationService.Navigated +=
                    OnTargetRegionNavigated;
                _regionManager.Regions.CollectionChanged -= OnRegionsCollectionChanged;
            }
        }

        private void OnTargetRegionNavigated(object sender, RegionNavigationEventArgs e)
        {
            _disregardNavigation = true;

            IRegionNavigationJournalEntry entry = e.NavigationContext.NavigationService.Journal.CurrentEntry;

            foreach (var navigationItem in _navigationItems)
            {
                navigationItem.IsSelected = IsFromThisItem(entry, navigationItem);
            }

            _disregardNavigation = false;
        }

        #endregion Methods
    }
}