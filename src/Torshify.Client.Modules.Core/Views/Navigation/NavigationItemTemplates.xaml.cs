using System.Windows;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public partial class NavigationItemTemplates : ResourceDictionary
    {
        public static readonly NavigationItemTemplates Instance = new NavigationItemTemplates();

        public NavigationItemTemplates()
        {
            InitializeComponent();
        }
    }
}