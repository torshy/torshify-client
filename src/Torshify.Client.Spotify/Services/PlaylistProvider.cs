using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Logging;
using Torshify.Client.Infrastructure.Interfaces;

using ITorshifyPlaylist = Torshify.Client.Infrastructure.Interfaces.IPlaylist;

namespace Torshify.Client.Spotify.Services
{
    public class PlaylistProvider : IPlaylistProvider
    {
        #region Fields

        private readonly ISession _session;
        private readonly Dispatcher _dispatcher;
        private readonly ILoggerFacade _logger;

        private ObservableCollection<Playlist> _playlists;

        #endregion Fields

        #region Constructors

        public PlaylistProvider(
            ISession session, 
            Dispatcher dispatcher,
            ILoggerFacade logger)
        {
            _session = session;
            _dispatcher = dispatcher;
            _logger = logger;
            _playlists = new ObservableCollection<Playlist>();

            if (_session.PlaylistContainer != null)
            {
                InitializePlaylistContainer();
            }
            else
            {
                _session.LoginComplete += (s, e) =>
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

        public event EventHandler<Infrastructure.Interfaces.PlaylistMovedEventArgs> PlaylistMoved = delegate { };

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
            _logger.Log("Initializing playlist container", Category.Info, Priority.Medium);

            if (_session.PlaylistContainer.IsLoaded)
            {
                FetchPlaylists();

                _session.PlaylistContainer.PlaylistAdded += OnPlaylistContainerPlaylistAdded;
                _session.PlaylistContainer.PlaylistRemoved += OnPlaylistContainerPlaylistRemoved;
                _session.PlaylistContainer.PlaylistMoved += OnPlaylistContainerPlaylistMoved;

            }
            else
            {
                _session.PlaylistContainer.Loaded += OnPlaylistContainerLoaded;
            }
        }

        private void OnPlaylistContainerPlaylistMoved(object sender, PlaylistMovedEventArgs e)
        {
            _logger.Log("Playlist moved", Category.Info, Priority.Medium);
            ITorshifyPlaylist p = _playlists[e.OldIndex];
            OnPlaylistMoved(new Infrastructure.Interfaces.PlaylistMovedEventArgs(p, e.OldIndex, e.NewIndex));
        }

        private void OnPlaylistContainerPlaylistRemoved(object sender, PlaylistEventArgs e)
        {
            _logger.Log("Playlist removed", Category.Info, Priority.Medium);
            ITorshifyPlaylist p = _playlists[e.Position];
            OnPlaylistRemoved(new Infrastructure.Interfaces.PlaylistEventArgs(p, e.Position));
        }

        private void OnPlaylistContainerPlaylistAdded(object sender, PlaylistEventArgs e)
        {
            _logger.Log("Playlist added", Category.Info, Priority.Medium);
            //InsertAt(e.Playlist, e.Position);
        }

        private void OnPlaylistContainerLoaded(object sender, EventArgs e)
        {
            _logger.Log("Playlist container loaded", Category.Info, Priority.Medium);

            _session.PlaylistContainer.Loaded -= OnPlaylistContainerLoaded;
            _session.PlaylistContainer.PlaylistAdded += OnPlaylistContainerPlaylistAdded;
            _session.PlaylistContainer.PlaylistRemoved += OnPlaylistContainerPlaylistRemoved;
            _session.PlaylistContainer.PlaylistMoved += OnPlaylistContainerPlaylistMoved;

            FetchPlaylists();
        }

        private void FetchPlaylists()
        {
            _logger.Log("Fetching playlists...", Category.Info, Priority.Medium);

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
                _dispatcher.BeginInvoke((Action<IPlaylist, int>)InsertAt, playlist, position);
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