using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

using EchoNest;
using EchoNest.Artist;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Modules.EchoNest.Controls;

namespace Torshify.Client.Modules.EchoNest.Views.Similar
{
    public class SimilarArtistViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly Dispatcher _dispatcher;
        private NodeCollection<SimilarArtistModel> _nodes;

        #endregion Fields

        #region Constructors

        public SimilarArtistViewModel(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;

            StartTrailCommand = new StaticCommand<string>(ExecuteStartTrail);
            ChangeCenterCommand = new AutomaticCommand<Node<SimilarArtistModel>>(ExecuteChangeCenter, CanChangeCenter);
        }

        #endregion Constructors

        #region Properties

        public AutomaticCommand<Node<SimilarArtistModel>> ChangeCenterCommand
        {
            get;
            private set;
        }

        public Graph Graph
        {
            get;
            set;
        }

        public StaticCommand<string> StartTrailCommand
        {
            get;
            private set;
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
            StartTrailCommand.Execute("NOFX");
        }

        private bool CanChangeCenter(Node<SimilarArtistModel> node)
        {
            return node != Graph.CenterObject;
        }

        private void ExecuteChangeCenter(Node<SimilarArtistModel> node)
        {
            Graph.CenterObject = node;

            Task.Factory
                .StartNew(() => FindSimilarArtists(node));
        }

        private void ExecuteStartTrail(string artistName)
        {
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
                                                        ArtistBucket.Images);


                if (result.Status.Code == ResponseCode.Success)
                {
                    centerObject.Name = result.Artist.Name;
                    centerObject.Familiarity = result.Artist.Familiarity;
                    centerObject.Hotttnesss = result.Artist.Hotttnesss;
                    centerObject.ImageUrl =
                        result.Artist.Images != null && result.Artist.Images.Count > 0
                        ? result.Artist.Images.FirstOrDefault().Url
                        : null;
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
                argument.Bucket = ArtistBucket.Images | ArtistBucket.Hotttnesss | ArtistBucket.Familiarity;

                var result = session.Query<SimilarArtists>().Execute(argument);

                if (result.Status.Code == ResponseCode.Success)
                {
                    foreach (var artist in result.Artists)
                    {
                        var artistModel = new SimilarArtistModel();
                        artistModel.Name = artist.Name;
                        artistModel.ImageUrl = artist.Images != null && artist.Images.Count > 0
                                             ? artist.Images.FirstOrDefault().Url
                                             : null;
                        artistModel.Familiarity = artist.Familiarity;
                        artistModel.Hotttnesss = artist.Hotttnesss;

                        _dispatcher.BeginInvoke(new Action(() =>
                                                               {
                                                                   if (!_nodes.HasNode(artistModel))
                                                                   {
                                                                       _nodes.Add(artistModel);
                                                                   }

                                                                   Node<SimilarArtistModel> other = _nodes[artistModel];
                                                                   if (!node.ChildNodes.Contains(other))
                                                                   {
                                                                       _nodes.AddEdge(artistModel, node.Item);
                                                                   }
                                                               }));
                    }
                }
            }

            return node;
        }

        #endregion Methods
    }
}