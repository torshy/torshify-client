using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Collections;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Artist.Tabs
{
    public class OverviewTabItemViewModel : NotificationObject, ITabViewModel<IArtist>
    {
        #region Fields

        private SortedObservableCollection<IAlbum> _albums;
        private ICollectionView _albumsIcv;
        private IArtist _artist;
        private IEventAggregator _eventAggregator;
        private SubscriptionToken _trackMenuBarToken;
        private SubscriptionToken _tracksMenuBarToken;

        #endregion Fields

        #region Constructors

        public OverviewTabItemViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #endregion Constructors

        #region Properties

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

        public string Header
        {
            get
            {
                return "Overview";
            }
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

            Albums = null;
            Artist.Info.FinishedLoading -= OnInfoFinishedLoading;
            Artist = null;

            _albums.Clear();
        }

        public void Initialize(NavigationContext navContext)
        {
            _trackMenuBarToken = _eventAggregator.GetEvent<TrackCommandBarEvent>().Subscribe(OnTrackMenuBarEvent, true);
            _tracksMenuBarToken = _eventAggregator.GetEvent<TracksCommandBarEvent>().Subscribe(OnTracksMenuBarEvent, true);
        }

        public void SetModel(IArtist model)
        {
            Artist = model;

            if (model.Info.IsLoading)
            {
                model.Info.FinishedLoading += OnInfoFinishedLoading;
            }
            else
            {
                PrepareData();
            }
        }

        private void OnInfoFinishedLoading(object sender, EventArgs e)
        {
            PrepareData();
        }

        private void OnTrackMenuBarEvent(TrackCommandBarModel model)
        {
            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, model.Track)
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Track);
        }

        private void OnTracksMenuBarEvent(TracksCommandBarModel model)
        {
            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, model.Tracks.LastOrDefault())
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Tracks);
        }

        private void PrepareData()
        {
            _albums = new SortedObservableCollection<IAlbum>(new AlbumComparer());

            Albums = CollectionViewSource.GetDefaultView(_albums);

            lock (_artist.Info.Albums.SyncRoot)
            {
                foreach (var album in _artist.Info.Albums)
                {
                    if (album.IsAvailable)
                    {
                        _albums.Add(album);
                    }
                }
            }
        }

        #endregion Methods

        #region Nested Types

        private class AlbumComparer : IComparer<IAlbum>
        {
            #region Methods

            public int Compare(IAlbum x, IAlbum y)
            {
                var xType = x.Type;
                var yType = y.Type;

                if (xType == yType)
                {
                    //return x.Year.CompareTo(y.Year) * -1;
                    return 0;
                }

                if (xType == AlbumType.Album)
                {
                    if (yType != AlbumType.Album)
                    {
                        return -1;
                    }
                }

                if (xType == AlbumType.Compilation)
                {
                    if (yType == AlbumType.Album || yType == AlbumType.Single)
                    {
                        return 1;
                    }

                    return -1;
                }

                if (xType == AlbumType.Single)
                {
                    if (yType != AlbumType.Album)
                    {
                        return -1;
                    }

                    return 1;
                }

                return 0;
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}