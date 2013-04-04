using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using EchoNest;
using EchoNest.Artist;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;

using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Modules.Core;
using Torshify.Client.Modules.EchoNest.Controls;

namespace Torshify.Client.Modules.EchoNest.Views.Similar
{
    [Export(typeof(SimilarArtistViewModel))]
    public class SimilarArtistViewModel : NotificationObject, INavigationAware, ITabViewModel<IArtist>
    {
        #region Fields

        private readonly Dispatcher _dispatcher;

        private IArtist _currentArtist;
        private Lazy<bool> _loadSimilar;
        private NodeCollection<SimilarArtistModel> _nodes;
        private object _nodesLock = new object();

        #endregion Fields

        #region Constructors

        public SimilarArtistViewModel()
        {
            _dispatcher = Application.Current.Dispatcher;

            StartTrailCommand = new StaticCommand<string>(ExecuteStartTrail);
            ChangeCenterCommand = new AutomaticCommand<Node<SimilarArtistModel>>(ExecuteChangeCenter, CanChangeCenter);
            GoToArtistCommand = new AutomaticCommand<Node<SimilarArtistModel>>(ExecuteGoToArtist, CanGoToArtist);
        }

        #endregion Constructors

        #region Properties

        public AutomaticCommand<Node<SimilarArtistModel>> ChangeCenterCommand
        {
            get;
            private set;
        }

        public AutomaticCommand<Node<SimilarArtistModel>> GoToArtistCommand
        {
            get;
            private set;
        }

        public Graph Graph
        {
            get;
            set;
        }

        public string Header
        {
            get { return "Discover Others"; }
        }

        public StaticCommand<string> StartTrailCommand
        {
            get;
            private set;
        }

        public Visibility Visibility
        {
            get { return Visibility.Visible; }
        }

        #endregion Properties

        #region Methods

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        void ITabViewModel<IArtist>.Deinitialize(NavigationContext navContext)
        {
            Graph.CenterObject = null;
            _nodes = new NodeCollection<SimilarArtistModel>(new List<SimilarArtistModel>());
            _currentArtist = null;
            _loadSimilar = null;
        }

        void ITabViewModel<IArtist>.Initialize(NavigationContext navContext)
        {
        }

        void ITabViewModel<IArtist>.SetModel(IArtist model)
        {
            _currentArtist = model;
            _loadSimilar = new Lazy<bool>(() =>
                                              {
                                                  ExecuteStartTrail(_currentArtist.Name);
                                                  return true;
                                              });
        }

        public void NavigatedFrom()
        {
        }

        public void NavigatedTo()
        {
            _loadSimilar.Value.ToString();
        }

        private bool CanChangeCenter(Node<SimilarArtistModel> node)
        {
            return node != Graph.CenterObject;
        }

        private bool CanGoToArtist(Node<SimilarArtistModel> artistNode)
        {
            return true;
        }

        private void ExecuteChangeCenter(Node<SimilarArtistModel> node)
        {
            Graph.CenterObject = node;

            Task.Factory.StartNew(() => FindSimilarArtists(node));
        }

        private void ExecuteGoToArtist(Node<SimilarArtistModel> artistNode)
        {
            var searchProvider = ServiceLocator.Current.TryResolve<ISearchProvider>();

            if (searchProvider != null)
            {
                var search = searchProvider.Search(artistNode.Item.Name, 0, int.MaxValue, 0, 10, 0, 10);
                search.FinishedLoading += SearchFinished;
            }
        }

        private void ExecuteStartTrail(string artistName)
        {
            if (Graph.CenterObject != null)
            {
                if (((Node<SimilarArtistModel>)Graph.CenterObject).Item.Name == artistName)
                {
                    return;
                }
            }

            _nodes = new NodeCollection<SimilarArtistModel>(new List<SimilarArtistModel>());

            Task<Node<SimilarArtistModel>>
                .Factory
                .StartNew(FindRootArtistInformation, artistName)
                .ContinueWith(t => FindSimilarArtists(t.Result))
                .ContinueWith(t => _dispatcher.BeginInvoke(new Action(() => Graph.CenterObject = t.Result)));
        }

        private Node<SimilarArtistModel> FindRootArtistInformation(object artistNameParameter)
        {
            string artistName = artistNameParameter.ToString();

            SimilarArtistModel centerObject = new SimilarArtistModel();

            using (var session = new EchoNestSession(EchoNestModule.ApiKey))
            {
                var result = session.Query<Profile>().Execute(
                                                        artistName,
                                                        ArtistBucket.Hotttnesss |
                                                        ArtistBucket.Familiarity |
                                                        ArtistBucket.Images |
                                                        ArtistBucket.Terms);

                if (result.Status.Code == ResponseCode.Success)
                {
                    centerObject.Name = result.Artist.Name;
                    centerObject.Familiarity = result.Artist.Familiarity.GetValueOrDefault();
                    centerObject.Hotttnesss = result.Artist.Hotttnesss.GetValueOrDefault();
                    centerObject.ImageUrl =
                        result.Artist.Images != null && result.Artist.Images.Count > 0
                        ? result.Artist.Images.FirstOrDefault().Url
                        : null;
                    centerObject.Terms = result.Artist.Terms != null ? result.Artist.Terms.Take(3) : null;
                }
                else
                {
                    centerObject.Name = "Unable to find what you searched for";
                    centerObject.Familiarity = 1.0;
                    centerObject.Hotttnesss = 1.0;
                }
            }

            _nodes.Add(centerObject);
            return _nodes[centerObject];
        }

        private Node<SimilarArtistModel> FindSimilarArtists(Node<SimilarArtistModel> node)
        {
            using (var session = new EchoNestSession(EchoNestModule.ApiKey))
            {
                SimilarArtistsArgument argument = new SimilarArtistsArgument();
                argument.Name = node.Item.Name;
                argument.Results = 9;
                argument.Bucket = ArtistBucket.Images | ArtistBucket.Hotttnesss | ArtistBucket.Familiarity | ArtistBucket.Terms;

                var result = session.Query<SimilarArtists>().Execute(argument);

                if (result.Status.Code == ResponseCode.Success)
                {
                    var list = new List<SimilarArtistModel>();

                    foreach (var artist in result.Artists)
                    {
                        var artistModel = new SimilarArtistModel();
                        artistModel.Name = artist.Name;
                        artistModel.ImageUrl = artist.Images != null && artist.Images.Count > 0
                                             ? artist.Images.FirstOrDefault().Url
                                             : null;
                        artistModel.Familiarity = artist.Familiarity.GetValueOrDefault();
                        artistModel.Hotttnesss = artist.Hotttnesss.GetValueOrDefault();
                        artistModel.Terms = artist.Terms != null ? artist.Terms.Take(3) : null;
                        list.Add(artistModel);
                    }

                    _dispatcher.BeginInvoke(new Action(() =>
                    {
                        lock (_nodesLock)
                        {
                            foreach (var artistModel in list)
                            {
                                if (!_nodes.HasNode(artistModel))
                                {
                                    _nodes.Add(artistModel);
                                }

                                Node<SimilarArtistModel> other = _nodes[artistModel];
                                if (!node.ChildNodes.Contains(other) && _nodes.HasNode(node.Item))
                                {
                                    _nodes.AddEdge(artistModel, node.Item);
                                }
                            }
                        }
                    }));
                }
            }

            return node;
        }

        private void SearchFinished(object sender, EventArgs e)
        {
            ISearch search = (ISearch)sender;
            search.FinishedLoading -= SearchFinished;

            if (search.TotalArtists > 0)
            {
                CoreCommands.Views.GoToArtistCommand.Execute(search.Artists.FirstOrDefault());
            }
        }

        #endregion Methods
    }
}