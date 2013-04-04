using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Artist.Tabs
{
    public class OverviewTabItemViewModel : NotificationObject, ITabViewModel<IArtist>
    {
        #region Fields

        private readonly Dispatcher _dispatcher;

        private ObservableCollection<IAlbum> _albums = new ObservableCollection<IAlbum>();
        private ICollectionView _albumsIcv;
        private IArtist _artist;
        private IArtistInformation _artistInformation;
        private IEventAggregator _eventAggregator;
        private SubscriptionToken _trackMenuBarToken;
        private SubscriptionToken _tracksMenuBarToken;

        #endregion Fields

        #region Constructors

        public OverviewTabItemViewModel(IEventAggregator eventAggregator, IPlayerController player, Dispatcher dispatcher)
        {
            _eventAggregator = eventAggregator;
            _dispatcher = dispatcher;

            Player = player;
            PlayArtistTrackCommand = new StaticCommand<ITrack>(ExecutePlayArtistTrack);
        }

        #endregion Constructors

        #region Properties

        public ICollectionView Albums
        {
            get
            {
                return _albumsIcv;
            }
            private set
            {
                _albumsIcv = value;
                RaisePropertyChanged("Albums");
            }
        }

        public IArtist Artist
        {
            get
            {
                return _artist;
            }
            private set
            {
                _artist = value;
                RaisePropertyChanged("Artist");
            }
        }

        public string Header
        {
            get
            {
                return Artist.Name;
            }
        }

        public ICommand PlayArtistTrackCommand
        {
            get;
            private set;
        }

        public IPlayerController Player
        {
            get; private set;
        }

        public Visibility Visibility
        {
            get { return Visibility.Visible; }
        }

        #endregion Properties

        #region Methods

        public void Deinitialize(NavigationContext navContext)
        {
            _eventAggregator.GetEvent<TrackCommandBarEvent>().Unsubscribe(_trackMenuBarToken);
            _eventAggregator.GetEvent<TracksCommandBarEvent>().Unsubscribe(_tracksMenuBarToken);

            _albums = null;
            _albumsIcv = null;
            _artist = null;

            if (_artistInformation != null)
            {
                _artistInformation.FinishedLoading -= OnInfoFinishedLoading;
                _artistInformation = null;
            }

            RaisePropertyChanged(String.Empty);
        }

        public void Initialize(NavigationContext navContext)
        {
            _trackMenuBarToken = _eventAggregator.GetEvent<TrackCommandBarEvent>().Subscribe(OnTrackMenuBarEvent, true);
            _tracksMenuBarToken = _eventAggregator.GetEvent<TracksCommandBarEvent>().Subscribe(OnTracksMenuBarEvent, true);
        }

        public void NavigatedFrom()
        {
        }

        public void NavigatedTo()
        {
        }

        public void SetModel(IArtist model)
        {
            Artist = model;

            _artistInformation = model.Info;

            if (_artistInformation.IsLoading)
            {
                _artistInformation.FinishedLoading += OnInfoFinishedLoading;
            }
            else
            {
                _dispatcher.BeginInvoke(new Action<IEnumerable<IAlbum>>(PrepareData), DispatcherPriority.Background, _artistInformation.Albums);
            }

            RaisePropertyChanged("Header");
        }

        private void ExecutePlayArtistTrack(ITrack track)
        {
            IEnumerable<ITrack> tracksToPlay = GetTracksToPlay(track);
            CoreCommands.PlayTrackCommand.Execute(tracksToPlay);
        }

        private IEnumerable<ITrack> GetTracksToPlay(ITrack track)
        {
            // This isn't working 100% because of the UI virtualization, and the caching/cleanup method i'm using for the album information.
            // This will only queue up what the view has created, which probably is just a fraction of certain artists' tracks.
            // TODO : Figure it out

            List<ITrack> tracksToPlay = new List<ITrack>();
            bool addRest = false;
            foreach (var album in _albums)
            {
                int index = album.Info.Tracks.IndexOf(track);

                if (index != -1 && addRest == false)
                {
                    tracksToPlay.AddRange(album.Info.Tracks.Skip(index));
                    addRest = true;
                    continue;
                }

                if (addRest)
                {
                    tracksToPlay.AddRange(album.Info.Tracks);
                }
            }
            return tracksToPlay;
        }

        private void OnAlbumInfoLoaded(object sender, AlbumInformationEventArgs e)
        {
            IAlbumInformation info = (IAlbumInformation) sender;
            info.Loaded -= OnAlbumInfoLoaded;

            if (Albums != null)
            {
                _dispatcher.BeginInvoke(new Action<IAlbum>(_albums.Add), DispatcherPriority.Background, e.Album);
            }
        }

        private void OnInfoFinishedLoading(object sender, EventArgs e)
        {
            IArtistInformation info = (IArtistInformation) sender;
            info.FinishedLoading -= OnInfoFinishedLoading;

            _dispatcher.BeginInvoke(new Action<IEnumerable<IAlbum>>(PrepareData), DispatcherPriority.Background, info.Albums);
        }

        private void OnTrackMenuBarEvent(TrackCommandBarModel model)
        {
            IEnumerable<ITrack> tracksToPlay = GetTracksToPlay(model.Track);

            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, tracksToPlay)
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Track);
        }

        private void OnTracksMenuBarEvent(TracksCommandBarModel model)
        {
            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, model.Tracks.LastOrDefault())
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Tracks);
        }

        private void PrepareData(IEnumerable<IAlbum> albums)
        {
            _albums = new ObservableCollection<IAlbum>();

            foreach (var album in albums)
            {
                if (album.IsAvailable)
                {
                    if (album.Info.IsLoading)
                    {
                        album.Info.Loaded += OnAlbumInfoLoaded;
                    }
                    else
                    {
                        _albums.Add(album);
                    }
                }
            }

            var icv = new ListCollectionView(_albums);
            icv.CustomSort = new AlbumComparer();
            Albums = icv;
        }

        #endregion Methods

        #region Nested Types

        private class AlbumComparer : IComparer<IAlbum>, IComparer
        {
            #region Methods

            public int Compare(IAlbum x, IAlbum y)
            {
                int result = x.Type.CompareTo(y.Type);

                if (result == 0)
                {
                    result = y.Year.CompareTo(x.Year);
                }

                return result;
            }

            public int Compare(object x, object y)
            {
                return Compare(x as IAlbum, y as IAlbum);
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}