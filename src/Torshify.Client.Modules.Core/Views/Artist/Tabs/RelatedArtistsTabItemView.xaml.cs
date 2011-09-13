using System;
using System.Windows.Controls;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Artist.Tabs
{
    public partial class RelatedArtistsTabItemView : UserControl, ITab<IArtist>
    {
        #region Constructors

        public RelatedArtistsTabItemView(RelatedArtistsTabItemViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
        }

        #endregion Constructors

        #region Properties

        public ITabViewModel<IArtist> ViewModel
        {
            get { return DataContext as RelatedArtistsTabItemViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}