using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Artist
{
    public class ArtistViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private IArtist _artist;
        private ITrack _selectedTrack;

        #endregion Fields

        #region Properties

        public IArtist Artist
        {
            get
            {
                return _artist;
            }
            set
            {
                if (_artist != value)
                {
                    _artist = value;
                    RaisePropertyChanged("Artist");
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
            Artist = navigationContext.Tag as IArtist;
        }

        #endregion Methods
    }
}