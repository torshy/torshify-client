using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using ITorshifyAlbum = Torshify.Client.Infrastructure.Interfaces.IAlbum;

using ITorshifyArtist = Torshify.Client.Infrastructure.Interfaces.IArtist;

using ITorshifyTrack = Torshify.Client.Infrastructure.Interfaces.ITrack;

namespace Torshify.Client.Spotify.Services
{
    public class Track : NotificationObject, ITorshifyTrack
    {
        #region Fields

        private readonly Dispatcher _dispatcher;

        private Lazy<Album> _album;
        private Lazy<IEnumerable<Artist>> _artists;

        #endregion Fields

        #region Constructors

        public Track(ITrack track, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            InternalTrack = track;

            _album = new Lazy<Album>(() => new Album(InternalTrack.Album, dispatcher));
            _artists = new Lazy<IEnumerable<Artist>>(() => InternalTrack.Artists.Select(artist => new Artist(artist)));
        }

        #endregion Constructors

        #region Properties

        public ITorshifyAlbum Album
        {
            get { return _album.Value; }
        }

        public IEnumerable<ITorshifyArtist> Artists
        {
            get { return _artists.Value; }
        }

        public int Disc
        {
            get { return InternalTrack.Disc; }
        }

        public TimeSpan Duration
        {
            get { return InternalTrack.Duration; }
        }

        public int ID
        {
            get { return InternalTrack.GetHashCode(); }
        }

        public int Index
        {
            get { return InternalTrack.Index; }
        }

        public ITrack InternalTrack
        {
            get;
            private set;
        }

        public bool IsAvailable
        {
            get { return InternalTrack.IsAvailable; }
        }

        public bool IsStarred
        {
            get { return InternalTrack.IsStarred; }
            set
            {
                if (InternalTrack.IsStarred != value)
                {
                    InternalTrack.IsStarred = value;
                    RaisePropertyChanged("IsStarred");
                }
            }
        }

        public string Name
        {
            get { return InternalTrack.Name; }
        }

        public int Popularity
        {
            get { return InternalTrack.Popularity; }
        }

        #endregion Properties
    }
}