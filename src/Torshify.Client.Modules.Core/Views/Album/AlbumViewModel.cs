using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows.Data;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;

using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Modules.Core.Views.Album.Tabs;

namespace Torshify.Client.Modules.Core.Views.Album
{
    public class AlbumViewModel : NotificationObject, INavigationAware, IPartImportsSatisfiedNotification
    {
        #region Fields

        [ImportMany]
        private IEnumerable<Lazy<ITab<IAlbum>>> _tabImports = null;
        private readonly ObservableCollection<ITab<IAlbum>> _tabs;
        private readonly ICollectionView _tabsIcv;

        #endregion Fields

        #region Constructors

        public AlbumViewModel(CompositionContainer mefContainer)
        {
            _tabs = new ObservableCollection<ITab<IAlbum>>();
            _tabs.Add(ServiceLocator.Current.TryResolve<AlbumTabItemView>());
            _tabsIcv = new ListCollectionView(_tabs);

            mefContainer.SatisfyImportsOnce(this);
        }

        #endregion Constructors

        #region Properties

        public ICollectionView Tabs
        {
            get
            {
                return _tabsIcv;
            }
        }

        #endregion Properties

        #region Methods

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            foreach (var tabItem in _tabs)
            {
                tabItem.ViewModel.Deinitialize(navigationContext);
            }
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            var artist = navigationContext.Tag as IAlbum;

            foreach (var tabItem in _tabs)
            {
                tabItem.ViewModel.SetModel(artist);
                tabItem.ViewModel.Initialize(navigationContext);
            }

            Tabs.MoveCurrentToFirst();
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            foreach (var tabImport in _tabImports)
            {
                _tabs.Add(tabImport.Value);
            }
        }

        #endregion Methods
    }
}