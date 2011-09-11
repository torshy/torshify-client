using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Modules.Core;
using Torshify.Client.Spotify.Services;

namespace Torshify.Client.Spotify
{
    public class SpotifyLinkNavigator : IInitializable
    {
        #region Fields

        private readonly ISession _session;

        #endregion Fields

        #region Constructors

        public SpotifyLinkNavigator(ISession session)
        {
            _session = session;
        }

        #endregion Constructors

        #region Methods

        public void Initialize()
        {
            Application.Current.MainWindow.AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(OnRequestNavigateFromUrl));
        }

        private void OnRequestNavigateFromUrl(object sender, RequestNavigateEventArgs e)
        {
            ILink link = _session.FromLink(e.Uri.OriginalString);

            if (link != null)
            {
                if (link is ILink<IArtist>)
                {
                    IArtist artist = ((ILink<IArtist>)link).Object;
                    Artist torshifyArtist = new Artist(artist, Application.Current.Dispatcher);
                    CoreCommands.Views.GoToArtistCommand.Execute(torshifyArtist);
                }
                else if (link is ILink<IAlbum>)
                {
                    IAlbum album = ((ILink<IAlbum>)link).Object;
                    Album torshifyAlbum = new Album(album, Application.Current.Dispatcher);
                    CoreCommands.Views.GoToAlbumCommand.Execute(torshifyAlbum);
                }
                else if (link is ILink<ITrackAndOffset>)
                {
                    ITrackAndOffset track = ((ILink<ITrackAndOffset>) link).Object;
                    Track torshifyTrack = new Track(track.Track, Application.Current.Dispatcher);
                    CoreCommands.PlayTrackCommand.Execute(torshifyTrack);
                    CoreCommands.Player.SeekCommand.Execute(track.Offset);
                }
            }
        }

        #endregion Methods
    }
}