using System;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Artist.Tabs
{
    public class BiographyTabItemViewModel : NotificationObject, ITabViewModel<IArtist>
    {
        #region Fields

        private Visibility _visibility;
        private string _biography;

        #endregion Fields

        #region Properties

        public string Header
        {
            get { return "Biography"; }
        }

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    RaisePropertyChanged("Visibility");
                }
            }
        }

        public string Biography
        {
            get { return _biography; }
            set
            {
                if (_biography != value)
                {
                    _biography = value;
                    RaisePropertyChanged("Biography");
                }
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
            if (model.Info.IsLoading)
            {
                model.Info.FinishedLoading += OnFinishedLoading;
            }
            else
            {
                PrepareData(model.Info);
            }
        }

        public void NavigatedTo()
        {
            
        }

        public void NavigatedFrom()
        {

        }

        private void OnFinishedLoading(object sender, EventArgs e)
        {
            IArtistInformation info = (IArtistInformation)sender;
            info.FinishedLoading -= OnFinishedLoading;
            PrepareData(info);
        }

        private void PrepareData(IArtistInformation artistInformation)
        {
            Biography = artistInformation.Biography;
            Visibility = string.IsNullOrEmpty(Biography)
                             ? Visibility.Collapsed
                             : Visibility.Visible;
        }

        #endregion Methods
    }
}