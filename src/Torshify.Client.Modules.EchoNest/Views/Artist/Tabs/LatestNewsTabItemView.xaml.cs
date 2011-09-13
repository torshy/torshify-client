using System.ComponentModel.Composition;
using System.Windows.Controls;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.EchoNest.Views.Artist.Tabs
{
    [Export(typeof(ITab<IArtist>))]
    public partial class LatestNewsTabItemView : UserControl, ITab<IArtist>
    {
        #region Constructors

        [ImportingConstructor]
        public LatestNewsTabItemView(LatestNewsTabItemViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        #endregion Constructors

        #region Properties

        public ITabViewModel<IArtist> ViewModel
        {
            get { return DataContext as LatestNewsTabItemViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}