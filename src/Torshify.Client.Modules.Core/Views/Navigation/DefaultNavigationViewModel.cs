using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Practices.Prism.Regions;
using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public class DefaultNavigationViewModel : NavigationViewModelBase<NavigationItem>
    {
        public DefaultNavigationViewModel(IRegionManager regionManager)
            : base(regionManager)
        {
            InitializeNavigationItems();
        }

        public ICollectionView Items
        {
            get;
            private set;
        }

        private void InitializeNavigationItems()
        {
            NavigationItems.Add(new HeaderedNavigationItem(new Uri(MusicRegionViewNames.WhatsNew, UriKind.Relative), "What's New"));
            NavigationItems.Add(new HeaderedNavigationItem(new Uri(MusicRegionViewNames.PlayQueueView, UriKind.Relative), "Play Queue"));
            NavigationItems.Add(new HeaderedNavigationItem(new Uri(MusicRegionViewNames.StarredView, UriKind.Relative), "Starred"));
            Items = new ListCollectionView(NavigationItems);
        }

        protected override bool CanNavigateToItem(NavigationItem item)
        {
            return true;
        }
    }

    public class DefaultNavigationViewIconTemplateSelector : DataTemplateSelector
    {
        public DataTemplate WhatsNew { get; set; }
        public DataTemplate PlayQueue { get; set; }
        public DataTemplate Starred { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            HeaderedNavigationItem navItem = item as HeaderedNavigationItem;

            if (navItem != null)
            {
                if (navItem.NavigationUrl.OriginalString == MusicRegionViewNames.WhatsNew)
                {
                    return WhatsNew;
                } 

                if (navItem.NavigationUrl.OriginalString == MusicRegionViewNames.PlayQueueView)
                {
                    return PlayQueue;
                }

                if (navItem.NavigationUrl.OriginalString == MusicRegionViewNames.StarredView)
                {
                    return Starred;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}