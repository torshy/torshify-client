using System;
using System.ComponentModel;
using System.Windows.Data;

using Microsoft.Practices.Prism.Regions;

using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.EchoNest.Views
{
    public class EchoNestNavigationViewModel : NavigationViewModelBase<NavigationItem>
    {
        #region Constructors

        public EchoNestNavigationViewModel(IRegionManager regionManager)
            : base(regionManager)
        {
            NavigationItems.Add(new HeaderedNavigationItem(new Uri(EchoNestViews.DiscoverMusicView, UriKind.Relative), "Discover music"));
            NavigationItems.Add(new HeaderedNavigationItem(new Uri(EchoNestViews.SimilarArtistView, UriKind.Relative), "Follow the trail"));
            Items = new ListCollectionView(NavigationItems);
        }

        #endregion Constructors

        #region Properties

        public ICollectionView Items
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        protected override bool CanNavigateToItem(NavigationItem item)
        {
            return true;
        }

        #endregion Methods
    }
}