using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;
using System.Linq;

namespace Torshify.Client.Modules.Core.Views.Artist.Tabs
{
    public class RelatedArtistsTabItemViewModel : NotificationObject, ITabViewModel<IArtist>
    {
        private IArtist _artist;
        private ICollectionView _similarArtistsIcv;

        #region Properties

        public string Header
        {
            get { return "Related Artists"; }
        }

        public Visibility Visibility
        {
            get { return Visibility.Visible; }
        }

        public IArtist Artist
        {
            get { return _artist; }
            set
            {
                if (_artist != value)
                {
                    _artist = value;
                    RaisePropertyChanged("Artist");
                }
            }
        }

        public ICollectionView SimilarArtists
        {
            get { return _similarArtistsIcv; }
            set
            {
                _similarArtistsIcv = value;
                RaisePropertyChanged("SimilarArtists");
            }
        }

        #endregion Properties

        #region Methods

        public void Deinitialize(NavigationContext navContext)
        {
        }

        public void Initialize(NavigationContext navContext)
        {
        }

        public void SetModel(IArtist model)
        {
            Artist = model;

            if (Artist.Info.IsLoading)
            {
                Artist.Info.FinishedLoading += OnInfoFinishedLoading;
            }
            else
            {
                PrepareData(Artist.Info);
            }
        }

        public void NavigatedTo()
        {
        }

        public void NavigatedFrom()
        {
        }

        private void OnInfoFinishedLoading(object sender, EventArgs e)
        {
            var artistInformation = (IArtistInformation) sender;
            artistInformation.FinishedLoading -= OnInfoFinishedLoading;
            PrepareData(artistInformation);
        }

        private void PrepareData(IArtistInformation artistInformation)
        {
            SimilarArtists = new ListCollectionView(artistInformation.SimilarArtists.ToArray());
        }

        #endregion Methods
    }
}