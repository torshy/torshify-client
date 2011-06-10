using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using ITorshifyPlaylist = Torshify.Client.Infrastructure.Interfaces.IPlaylist;

using ITorshifyPlaylistTrack = Torshify.Client.Infrastructure.Interfaces.IPlaylistTrack;

namespace Torshify.Client.Spotify.Services
{
    public class Playlist : NotificationObject, ITorshifyPlaylist
    {
        #region Fields

        private readonly Dispatcher _dispatcher;

        private string _name;
        private string _description;
        private ObservableCollection<PlaylistTrack> _tracks;

        #endregion Fields

        #region Constructors

        public Playlist(IPlaylist playlist, Dispatcher dispatcher)
        {
            InternalPlaylist = playlist;

            _dispatcher = dispatcher;
            _tracks = new ObservableCollection<PlaylistTrack>();
        }

        #endregion Constructors

        #region Properties

        public IPlaylist InternalPlaylist
        {
            get; private set;
        }

        public string Name
        {
            get
            {
                return InternalPlaylist.Name;
            }
            set
            {
                InternalPlaylist.Name = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public IEnumerable<ITorshifyPlaylistTrack> Tracks
        {
            get
            {
                return _tracks;
            }
        }

        #endregion Properties
    }
}