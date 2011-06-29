using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Album
{
    public class AlbumViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private IAlbum _album;

        #endregion Fields

        #region Properties

        public IAlbum Album
        {
            get
            {
                return _album;
            }
            set
            {
                if (_album != value)
                {
                    _album = value;
                    RaisePropertyChanged("Album");
                }
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
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Album = navigationContext.Tag as IAlbum;
        }

        #endregion Methods
    }
}