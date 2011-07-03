using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Interfaces;
using System.Linq;

namespace Torshify.Client.Modules.Core.Views.WhatsNew.Tabs
{
    public class WhatsNewTabItemViewModel : NotificationObject, ITabViewModel<object>
    {
        #region Fields

        private readonly Dispatcher _dispatcher;
        private readonly ISearchProvider _searchProvider;

        private ObservableCollection<IAlbum> _albums;
        private ICollectionView _albumsIcv;
        private Random _random;
        private List<ISearch> _searchList;

        #endregion Fields

        #region Constructors

        public WhatsNewTabItemViewModel(
            ISearchProvider searchProvider,
            Dispatcher dispatcher)
        {
            _random = new Random();
            _searchProvider = searchProvider;
            _searchList = new List<ISearch>();
            _dispatcher = dispatcher;

            NumberToFetchPerBatch = 9;
            GetMoreAlbumsCommand = new StaticCommand(GetMoreRandomAlbums);
        }

        #endregion Constructors

        #region Properties

        public ICollectionView Albums
        {
            get { return _albumsIcv; }
            set
            {
                _albumsIcv = value;
                RaisePropertyChanged("Albums");
            }
        }

        public ICommand GetMoreAlbumsCommand
        {
            get;
            private set;
        }

        public string Header
        {
            get { return "What's New"; }
        }

        public Visibility Visibility
        {
            get { return Visibility.Visible; }
        }

        public int NumberToFetchPerBatch
        {
            get; 
            set;
        }

        #endregion Properties

        #region Methods

        public void Deinitialize(NavigationContext navContext)
        {
            Albums = null;
            _searchList.Clear();
        }

        public void Initialize(NavigationContext navContext)
        {
            if (string.IsNullOrEmpty(navContext.Parameters["WhatsNewTag"]))
            {
                UriQuery query = new UriQuery();
                query.Add("WhatsNewTag", DateTime.Now.ToLongTimeString());
                navContext.NavigationService.Journal.CurrentEntry.Uri = new Uri(MusicRegionViewNames.WhatsNew + query,
                                                                                UriKind.Relative);

                _albums = new ObservableCollection<IAlbum>();
                Albums = new ListCollectionView(_albums);

                GetMoreRandomAlbums();
            }
            else
            {
                Albums = new ListCollectionView(_albums);
            }
        }

        public void SetModel(object model)
        {
        }

        private void GetMoreRandomAlbums()
        {
            // Ok ok so its not really "What's New". Its four years old random shite, but libspotify doesn't provide whats new.
            // Might swap this out with a nation-combined toplist thing instead later.
            var search = _searchProvider.Search(
                DateTime.Now.Year - 4,
                DateTime.Now.Year,
                Genre.All);

            search.FinishedLoading += OnSearchFinishedLoading;
            _searchList.Add(search);
        }

        private void OnSearchFinishedLoading(object sender, EventArgs e)
        {
            ISearch search = (ISearch) sender;
            search.FinishedLoading -= OnSearchFinishedLoading;

            int maxValue = search.Tracks.Count;

            List<IAlbum> toAdd = new List<IAlbum>();
            for (int i = 0; i < NumberToFetchPerBatch; i++)
            {
                int randomIndex = _random.Next(0, maxValue);
                ITrack randomTrack = search.Tracks[randomIndex];
                Func<IAlbum, bool> predicate = a => a.Name.Equals(randomTrack.Album.Name);

                if (toAdd.Any(predicate) || _albums.Any(predicate))
                {
                    i--;
                }
                else
                {
                    toAdd.Add(randomTrack.Album);
                }
            }

            foreach (var album in toAdd)
            {
                _dispatcher.BeginInvoke(
                    (Action<IAlbum>)_albums.Add,
                    DispatcherPriority.Background,
                    album);
            }

            _searchList.Remove(search);
        }

        #endregion Methods
    }
}