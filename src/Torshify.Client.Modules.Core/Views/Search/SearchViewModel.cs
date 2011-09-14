using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Search
{
    public class SearchViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly ISearchProvider _searchProvider;

        private SubscriptionToken _tracksMenuBarToken;
        private SubscriptionToken _trackMenuBarToken;
        private ISearch _currentSearch;
        private IList<ITrack> _searchResults;
        private ICollectionView _searchResultsIcv;
        private string _didYouMean;

        #endregion Fields

        #region Constructors

        public SearchViewModel(
            IEventAggregator eventAggregator, 
            ISearchProvider searchProvider,
            IPlayerController playerController,
            Dispatcher dispatcher)
        {
            _eventAggregator = eventAggregator;
            _searchProvider = searchProvider;
            _searchResults = new ObservableCollection<ITrack>();

            Player = playerController;
            PlayTrackCommand = new StaticCommand<ITrack>(ExecutePlayTrack);
        }

        #endregion Constructors

        #region Properties

        public ICollectionView SearchResults
        {
            get
            {
                return _searchResultsIcv;
            }
            private set
            {
                _searchResultsIcv = value;
                RaisePropertyChanged("SearchResults");
            }
        }

        public ICommand PlayTrackCommand
        {
            get;
            private set;
        }

        public IPlayerController Player
        {
            get;
            private set;
        }

        public string DidYouMean
        {
            get { return _didYouMean; }
            set
            {
                if (_didYouMean != value)
                {
                    _didYouMean = value;
                    RaisePropertyChanged("DidYouMean");
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
            _eventAggregator.GetEvent<TrackCommandBarEvent>().Unsubscribe(_trackMenuBarToken);
            _eventAggregator.GetEvent<TracksCommandBarEvent>().Unsubscribe(_tracksMenuBarToken);
            
            DidYouMean = string.Empty;

            if (_currentSearch != null)
            {
                _currentSearch.FinishedLoading -= SearchFinishedLoading;
            }

            SearchResults = null;
            _searchResults = null;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _trackMenuBarToken = _eventAggregator.GetEvent<TrackCommandBarEvent>().Subscribe(OnTrackMenuBarEvent, true);
            _tracksMenuBarToken = _eventAggregator.GetEvent<TracksCommandBarEvent>().Subscribe(OnTracksMenuBarEvent, true);

            DidYouMean = string.Empty;

            string query = navigationContext.Parameters["Query"];
            _currentSearch = _searchProvider.Search(query, 0, int.MaxValue, 0, 10, 0, 10);
            _currentSearch.FinishedLoading += SearchFinishedLoading;
        }

        private void SearchFinishedLoading(object sender, EventArgs e)
        {
            ISearch search = (ISearch)sender;
            search.FinishedLoading -= SearchFinishedLoading;

            DidYouMean = search.DidYouMean;

            _searchResults = new SearchResultsLoader(search.Query, search.Tracks, _searchProvider);
            SearchResults = new ListCollectionView((IList)_searchResults);
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

        private void ExecutePlayTrack(ITrack track)
        {
            // Get the rest of the tracks from the album, including the one selected.
            IEnumerable<ITrack> tracks = GetTracksToPlay(track);
            CoreCommands.PlayTrackCommand.Execute(tracks);
        }

        private IEnumerable<ITrack> GetTracksToPlay(ITrack track)
        {
            List<ITrack> itemsToPlay = new List<ITrack>();
            bool addItemToList = false;

            foreach (ITrack searchResult in SearchResults)
            {
                if (searchResult == track)
                {
                    addItemToList = true;
                }

                if (addItemToList)
                {
                    itemsToPlay.Add(searchResult);
                }
            }

            return itemsToPlay;
        }

        #endregion Methods

        #region Nested Types

        private class SearchResultsLoader : IList<ITrack>, IList, INotifyPropertyChanged, INotifyCollectionChanged
        {
            #region Fields

            public ISearch search;

            private readonly string _query;
            private readonly ISearchProvider _searchProvider;

            private bool _isFetching;
            private ObservableCollection<ITrack> _list;

            #endregion Fields

            #region Constructors

            public SearchResultsLoader(string query, IEnumerable<ITrack> initialResult, ISearchProvider searchProvider)
            {
                _list = new ObservableCollection<ITrack>(initialResult);

                _query = query;
                _searchProvider = searchProvider;
            }

            #endregion Constructors

            #region Events

            public event NotifyCollectionChangedEventHandler CollectionChanged
            {
                add { _list.CollectionChanged += value; }
                remove { _list.CollectionChanged -= value; }
            }

            public event PropertyChangedEventHandler PropertyChanged
            {
                add { (_list as INotifyPropertyChanged).PropertyChanged += value; }
                remove { (_list as INotifyPropertyChanged).PropertyChanged -= value; }
            }

            #endregion Events

            #region Properties

            public int Count
            {
                get { return _list.Count; }
            }

            public bool IsFixedSize
            {
                get { return false; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool IsSynchronized
            {
                get { return false; }
            }

            public object SyncRoot
            {
                get { return this; }
            }

            object IList.this[int index]
            {
                get { return this[index]; }
                set { throw new NotImplementedException(); }
            }

            #endregion Properties

            #region Indexers

            public ITrack this[int index]
            {
                get
                {
                    if (index >= (_list.Count - 1) && !_isFetching)
                    {
                        GetMore();
                    }

                    ITrack track = _list[index];
                    return track;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            #endregion Indexers

            #region Methods

            public void Add(ITrack item)
            {
                throw new NotImplementedException();
            }

            public int Add(object value)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(object value)
            {
                return _list.Contains((ITrack) value);
            }

            public bool Contains(ITrack item)
            {
                return _list.Contains(item);
            }

            public void CopyTo(ITrack[] array, int arrayIndex)
            {
                array.CopyTo(_list.ToArray(), arrayIndex);
            }

            public void CopyTo(Array array, int index)
            {
                array.CopyTo(_list.ToArray(), index);
            }

            public IEnumerator<ITrack> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int IndexOf(object value)
            {
                return _list.IndexOf((ITrack)value);
            }

            public int IndexOf(ITrack item)
            {
                return _list.IndexOf(item);
            }

            public void Insert(int index, object value)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, ITrack item)
            {
                throw new NotImplementedException();
            }

            public void Remove(object value)
            {
                throw new NotImplementedException();
            }

            public bool Remove(ITrack item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            private void FillCache(IEnumerable<ITrack> tracks)
            {
                foreach (var track in tracks)
                {
                    _list.Add(track);
                }
            }

            private void GetMore()
            {
                _isFetching = true;
                search =_searchProvider.Search(_query, _list.Count, int.MaxValue, 0, int.MaxValue, 0, int.MaxValue);
                search.FinishedLoading += search_FinishedLoading;
            }

            void search_FinishedLoading(object sender, EventArgs e)
            {
                FillCache(search.Tracks);

                _isFetching = false;
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}