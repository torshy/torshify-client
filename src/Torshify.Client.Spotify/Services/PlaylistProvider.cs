using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

using Torshify.Client.Infrastructure.Interfaces;

using ITorshifyPlaylist = Torshify.Client.Infrastructure.Interfaces.IPlaylist;
using System.Linq;

namespace Torshify.Client.Spotify.Services
{
    public class PlaylistProvider : IPlaylistProvider
    {
        #region Fields

        private readonly ISession _session;
        private readonly Dispatcher _dispatcher;

        private ObservableCollection<Playlist> _playlists;

        #endregion Fields

        #region Constructors

        public PlaylistProvider(ISession session, Dispatcher dispatcher)
        {
            _session = session;
            _dispatcher = dispatcher;
            _playlists = new ObservableCollection<Playlist>();

            if (_session.PlaylistContainer != null)
            {
                InitializePlaylistContainer();
            }
            else
            {
                _session.LoginComplete += (s,e)=>
                                              {
                                                  if (e.Status == Error.OK)
                                                  {
                                                      InitializePlaylistContainer();
                                                  }
                                              };
            }
        }

        #endregion Constructors

        #region Events

        public event EventHandler<Infrastructure.Interfaces.PlaylistEventArgs> PlaylistAdded = delegate { };

        public event EventHandler<Infrastructure.Interfaces.PlaylistEventArgs> PlaylistRemoved = delegate { };

        public event EventHandler<Infrastructure.Interfaces.PlaylistMovedEventArgs> PlaylistMoved;

        #endregion Events

        #region Properties

        public IEnumerable<ITorshifyPlaylist> Playlists
        {
            get
            {
                return _playlists;
            }
        }

        #endregion Properties

        #region Private Methods

        private void InitializePlaylistContainer()
        {
            _session.PlaylistContainer.PlaylistAdded += OnPlaylistContainerPlaylistAdded;
            _session.PlaylistContainer.PlaylistRemoved += OnPlaylistContainerPlaylistRemoved;
            _session.PlaylistContainer.PlaylistMoved += OnPlaylistContainerPlaylistMoved;

            if (_session.PlaylistContainer.IsLoaded)
            {
                FetchPlaylists();
            }
            else
            {
                _session.PlaylistContainer.Loaded += OnPlaylistContainerLoaded;
            }
        }

        private void OnPlaylistContainerPlaylistMoved(object sender, PlaylistMovedEventArgs e)
        {
            ITorshifyPlaylist p = _playlists.FirstOrDefault(i => i.InternalPlaylist == e.Playlist);

            if (p != null)
            {
                OnPlaylistMoved(new Infrastructure.Interfaces.PlaylistMovedEventArgs(p, e.OldIndex, e.NewIndex));
            }
        }

        private void OnPlaylistContainerPlaylistRemoved(object sender, PlaylistEventArgs e)
        {
            ITorshifyPlaylist p = _playlists.FirstOrDefault(i => i.InternalPlaylist == e.Playlist);

            if (p != null)
            {
                OnPlaylistRemoved(new Infrastructure.Interfaces.PlaylistEventArgs(p, e.Position));
            }
        }

        private void OnPlaylistContainerPlaylistAdded(object sender, PlaylistEventArgs e)
        {
            //InsertAt(e.Playlist, e.Position);
        }

        private void OnPlaylistContainerLoaded(object sender, EventArgs e)
        {
            _session.PlaylistContainer.Loaded -= OnPlaylistContainerLoaded;
            FetchPlaylists();
        }

        private void FetchPlaylists()
        {
            for (int i = 0; i < _session.PlaylistContainer.Playlists.Count; i++)
            {
                var p = _session.PlaylistContainer.Playlists[i];
                InsertAt(p, i);
            }
        }

        private void InsertAt(IPlaylist playlist, int position)
        {
            if (_dispatcher.CheckAccess())
            {
                var item = new Playlist(playlist, _dispatcher);
                _playlists.Insert(position, item);
                OnPlaylistAdded(new Infrastructure.Interfaces.PlaylistEventArgs(item, position));
            }
            else
            {
                _dispatcher.BeginInvoke((Action<IPlaylist, int>) InsertAt, playlist, position);
            }
        }

        private void OnPlaylistAdded(Infrastructure.Interfaces.PlaylistEventArgs e)
        {
            var handler = PlaylistAdded;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnPlaylistRemoved(Infrastructure.Interfaces.PlaylistEventArgs e)
        {
            var handler = PlaylistRemoved;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnPlaylistMoved(Infrastructure.Interfaces.PlaylistMovedEventArgs e)
        {
            var handler = PlaylistMoved;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion Private Methods
    }
}