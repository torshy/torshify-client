using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Infrastructure.Models
{
    public abstract class TabViewModel<TViewModel> : NotificationObject, INavigationAware
    {
        #region Fields

        private ObservableCollection<ITab<TViewModel>> _tabs;
        private ICollectionView _tabsIcv;

        #endregion Fields

        #region Constructors

        protected TabViewModel()
        {
            _tabs = new ObservableCollection<ITab<TViewModel>>();
            _tabsIcv = new ListCollectionView(_tabs);
            _tabsIcv.CurrentChanged += OnCurrentTabChanged;
            _tabsIcv.CurrentChanging += OnCurrentTabChanging;
        }

        #endregion Constructors

        #region Properties

        public ICollectionView Tabs
        {
            get { return _tabsIcv; }
        }

        #endregion Properties

        #region Methods

        public void AddTab(ITab<TViewModel> tab)
        {
            _tabs.Add(tab);
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            foreach (ITab<TViewModel> tabItem in Tabs)
            {
                tabItem.ViewModel.Deinitialize(navigationContext);
            }
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            TViewModel model = GetModel(navigationContext);

            foreach (ITab<TViewModel> tabItem in Tabs)
            {
                tabItem.ViewModel.Initialize(navigationContext);
                tabItem.ViewModel.SetModel(model);
            }

            Tabs.MoveCurrentToFirst();
        }

        public void RemoveTab(ITab<TViewModel> tab)
        {
            _tabs.Remove(tab);
        }

        protected abstract TViewModel GetModel(NavigationContext navigationContext);

        private void OnCurrentTabChanged(object sender, EventArgs e)
        {
            var tab = _tabsIcv.CurrentItem as ITab<TViewModel>;

            if (tab != null)
            {
                tab.ViewModel.NavigatedTo();
            }
        }

        private void OnCurrentTabChanging(object sender, CurrentChangingEventArgs e)
        {
            var tab = _tabsIcv.CurrentItem as ITab<TViewModel>;

            if (tab != null)
            {
                tab.ViewModel.NavigatedFrom();
            }
        }

        #endregion Methods
    }
}