using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;

using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Modules.Core.Views.WhatsNew.Tabs;

namespace Torshify.Client.Modules.Core.Views.WhatsNew
{
    public class WhatsNewViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private ObservableCollection<ITab<object>> _tabs;
        private ICollectionView _tabsIcv;

        #endregion Fields

        #region Constructors

        public WhatsNewViewModel()
        {
            _tabs = new ObservableCollection<ITab<object>>();
            _tabs.Add(ServiceLocator.Current.TryResolve<WhatsNewTabItemView>());
            _tabs.Add(ServiceLocator.Current.TryResolve<TopListsTabItemView>());
            _tabsIcv = new ListCollectionView(_tabs);
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

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            foreach (var tabItem in _tabs)
            {
                tabItem.ViewModel.Deinitialize(navigationContext);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            foreach (var tabItem in _tabs)
            {
                tabItem.ViewModel.SetModel(this);
                tabItem.ViewModel.Initialize(navigationContext);
            }

            Tabs.MoveCurrentToFirst();
        }

        #endregion Methods
    }
}