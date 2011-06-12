using System;

using Microsoft.Practices.Prism.ViewModel;

using ITorshifyAlbum = Torshify.Client.Infrastructure.Interfaces.IAlbum;

using ITorshifyArtist = Torshify.Client.Infrastructure.Interfaces.IArtist;

namespace Torshify.Client.Spotify.Services
{
    public class Album : NotificationObject, ITorshifyAlbum
    {
        #region Fields

        private Lazy<ITorshifyArtist> _artist;

        #endregion Fields

        #region Constructors

        public Album(IAlbum album)
        {
            InternalAlbum = album;

            _artist = new Lazy<ITorshifyArtist>(() => new Artist(InternalAlbum.Artist));
        }

        #endregion Constructors

        #region Properties

        public IAlbum InternalAlbum
        {
            get;
            private set;
        }

        public ITorshifyArtist Artist
        {
            get { return _artist.Value; }
        }

        public bool IsAvailable
        {
            get { return InternalAlbum.IsAvailable; }
        }

        public string Name
        {
            get { return InternalAlbum.Name; }
        }

        public int Year
        {
            get { return InternalAlbum.Year; }
        }

        #endregion Properties
    }
}