using System;
using Microsoft.Practices.Prism.Regions;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public class PlayQueueNavigationItem : INavigationItem
    {
        private readonly IRegionManager _regionManager;
        private Uri _uri;

        public PlayQueueNavigationItem(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _uri = new Uri(MusicRegionViewNames.PlayQueueView, UriKind.Relative);
        }

        public void NavigateTo()
        {
            _regionManager.RequestNavigate(CoreRegionNames.MainMusicRegion, _uri);
        }

        public bool IsMe(IRegionNavigationJournalEntry entry)
        {
            return entry.Uri == _uri;
        }
    }
}