using System;
using System.Windows;

using Microsoft.Practices.Prism.Regions;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public class PlayQueueNavigationItem : INavigationItem
    {
        #region Fields

        private readonly IRegionManager _regionManager;

        private Uri _uri;

        #endregion Fields

        #region Constructors

        public PlayQueueNavigationItem(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _uri = new Uri(MusicRegionViewNames.PlayQueueView, UriKind.Relative);
        }

        #endregion Constructors

        #region Properties

        public DataTemplate DataTemplate
        {
            get
            {
                return NavigationItemTemplates.Instance[GetType()] as DataTemplate;
            }
        }

        #endregion Properties

        #region Methods

        public bool IsMe(IRegionNavigationJournalEntry entry)
        {
            return entry.Uri == _uri;
        }

        public void NavigateTo()
        {
            _regionManager.RequestNavigate(CoreRegionNames.MainMusicRegion, _uri);
        }

        #endregion Methods
    }
}