using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Collections;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Artist
{
    public class ArtistViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private SortedObservableCollection<IAlbum> _albums;
        private ICollectionView _albumsIcv;
        private IArtist _artist;

        #endregion Fields

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
            Artist.Info.FinishedLoading -= OnInfoFinishedLoading;
            Artist = null;
            Albums = null;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Artist = navigationContext.Tag as IArtist;

            if (Artist != null)
            {
                Artist.Info.FinishedLoading += OnInfoFinishedLoading;
            }
        }

        private void OnInfoFinishedLoading(object sender, EventArgs e)
        {
            _albums = new SortedObservableCollection<IAlbum>(new AlbumComparer());

            Albums = CollectionViewSource.GetDefaultView(_albums);

            lock (Artist.Info.Albums.SyncRoot)
            {
                foreach (var album in Artist.Info.Albums)
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