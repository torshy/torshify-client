namespace Torshify.Client.Spotify.Views.Playlists
{
    public class UnknownPlaylistNavigationItem : PlaylistNavigationItem
    {
        #region Constructors

        public UnknownPlaylistNavigationItem(Infrastructure.Interfaces.IPlaylist playlist)
            : base(playlist)
        {
        }

        #endregion Constructors
    }
}