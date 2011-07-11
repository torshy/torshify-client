using System.Windows;
using System.Windows.Controls;

namespace Torshify.Client.Spotify.Views.Playlists
{
    public class PlaylistNavigationItemTemplateSelector : DataTemplateSelector
    {
        #region Properties

        public DataTemplate Folder
        {
            get; set;
        }

        public DataTemplate Playlist
        {
            get; set;
        }

        public DataTemplate Separator
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is FolderPlaylistNavigationItem)
            {
                return Folder;
            }

            if (item is PlaylistSeparatorNavigationItem)
            {
                return Separator;
            }

            return Playlist;
        }

        #endregion Methods
    }
}