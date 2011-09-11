using System.ComponentModel.Composition;
using System.Windows.Controls;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.EchoNest.Views.Similar
{
    [Export(typeof(ITab<IArtist>))]
    public partial class SimilarArtistView : UserControl, ITab<IArtist>
    {
        #region Constructors

        [ImportingConstructor]
        public SimilarArtistView(SimilarArtistViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
            Model.Graph = _graph;
        }

        #endregion Constructors

        #region Properties

        public SimilarArtistViewModel Model
        {
            get { return DataContext as SimilarArtistViewModel; }
            set { DataContext = value; }
        }

        public ITabViewModel<IArtist> ViewModel
        {
            get { return Model; }
        }

        #endregion Properties
    }
}