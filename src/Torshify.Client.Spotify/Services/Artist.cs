using Microsoft.Practices.Prism.ViewModel;

using ITorshifyArtist = Torshify.Client.Infrastructure.Interfaces.IArtist;

namespace Torshify.Client.Spotify.Services
{
    public class Artist : NotificationObject, ITorshifyArtist
    {
        #region Constructors

        public Artist(IArtist artist)
        {
            InternalArtist = artist;
        }

        #endregion Constructors

        #region Properties

        public IArtist InternalArtist
        {
            get; private set;
        }

        public string Name
        {
            get { return InternalArtist.Name; }
        }

        #endregion Properties
    }
}