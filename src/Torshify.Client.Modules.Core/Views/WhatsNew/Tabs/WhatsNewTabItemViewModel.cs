using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.WhatsNew.Tabs
{
    public class WhatsNewTabItemViewModel : NotificationObject, ITabViewModel<WhatsNewViewModel>
    {
        #region Fields

        private readonly Dispatcher _dispatcher;
        private readonly ISearchProvider _searchProvider;

        private ObservableCollection<IAlbum> _albums;
        private ICollectionView _albumsIcv;
        private Random _random;
        private List<ISearch> _searchList;
        private bool _isLoading;

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

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    RaisePropertyChanged("IsLoading");
                }
            }
        }
        #endregion Properties

        #region Methods

        public void Initialize(NavigationContext navContext)
        {
            _albums = new ObservableCollection<IAlbum>();
            Albums = new ListCollectionView(_albums);

            GetMoreRandomAlbums();
        }

        public void Deinitialize(NavigationContext navContext)
        {
            Albums = null;
            _albums = null;

            var deadSearches = new List<ISearch>();

            if (!IsLoading)
            {
                for (int i = 0; i < _searchList.Count; i++)
                {
                    if (!_searchList[i].IsLoading)
                    {
                        deadSearches.Add(_searchList[i]);
                    }
                }

                foreach (var deadSearch in deadSearches)
                {
                    _searchList.Remove(deadSearch);
                }
            }
        }

        public void SetModel(WhatsNewViewModel model)
        {
        }

        public void NavigatedTo()
        {
        }

        public void NavigatedFrom()
        {
        }

        private void GetMoreRandomAlbums()
        {
            if (!IsLoading)
            {
                IsLoading = true;

                var search = _searchProvider.Search("tag:new", 0, 0, 0, 250, 0, 0);

                search.FinishedLoading += OnSearchFinishedLoading;
                _searchList.Add(search);
            }
        }

        private void OnSearchFinishedLoading(object sender, EventArgs e)
        {
            ISearch search = (ISearch)sender;
            search.FinishedLoading -= OnSearchFinishedLoading;

            int maxValue = search.Albums.Count;

            if (maxValue > 0)
            {
                List<IAlbum> toAdd = new List<IAlbum>();
                for (int i = 0; i < NumberToFetchPerBatch; i++)
                {
                    int randomIndex = _random.Next(0, maxValue);
                    var random = search.Albums[randomIndex];
                    Func<IAlbum, bool> predicate = a => a.Name.Equals(random.Name);

                    if (toAdd.Any(predicate) || _albums.Any(predicate))
                    {
                        i--;
                    }
                    else
                    {
                        toAdd.Add(random);
                    }
                }

                foreach (var album in toAdd)
                {
                    _dispatcher.BeginInvoke(
                        (Action<IAlbum>)_albums.Add,
                        DispatcherPriority.Background,
                        album);
                }
            }

            _searchList.Remove(search);

            IsLoading = false;
        }

        #endregion Methods
    }
}