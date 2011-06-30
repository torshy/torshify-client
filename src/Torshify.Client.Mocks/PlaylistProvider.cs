using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using FizzWare.NBuilder;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class PlaylistProvider : IPlaylistProvider, IInitializable
    {
        #region Fields

        private IList<Playlist> _playlists;
        private Random _random;

        private string[] _playlistNames = {
                                              "Metal",
                                              "Book Of Heavy Metal",
                                              "Heavy",
                                              "Rock",
                                              "Punk",
                                              "Pop",
                                              "Absolute crap volume 1",
                                              "Absolute crap volume 2",
                                              "Absolute crap volume 3",
                                              "Absolute crap volume 4",
                                              "Random",
                                              "Chill",
                                              "New stuff",
                                              "Old stuff"
                                          };

        private string[] _artistNames = {
                                            "Arch Enemy",
                                            "Korn",
                                            "Machine Head",
                                            "Sepultura",
                                            "Chimaira",
                                            "As I Lay Dying",
                                            "Dimmu Borgir",
                                            "Extol",
                                            "In Flames",
                                            "Opeth",
                                            "Slayer",
                                            "Testament",
                                            "Vader"
                                        };

        private string[] _albumNames = {
                                           "Arise",
                                           "The Art Of War",
                                           "Blueprint",
                                           "The Burning Red",
                                           "Enemy Of God",
                                           "Fiction",
                                           "God Hates Us All",
                                           "Iowa",
                                           "Kvelertak",
                                           "L.D 50",
                                           "ObZen",
                                           "Prevail",
                                           "Roots",
                                           "The Unborn",
                                           "Wages Of Sin",
                                           "Xxv"
                                       };

        private string[] _trackNames = {
                                           "Bleed",
                                           "Backbone",
                                           "Blodtørst",
                                           "Davidian",
                                           "Desire",
                                           "Destroyer",
                                           "Dragula",
                                           "Dusted",
                                           "End Of The Line",
                                           "Fight Fire With Fire",
                                           "Flyhigh",
                                           "Happy?",
                                           "I Am Loco",
                                           "Laid To Rest",
                                           "Left Behind",
                                           "My Curse",
                                           "My Plague",
                                           "Peep Show",
                                           "Resurrection",
                                           "The Flame",
                                           "This Is War",
                                           "Trigger",
                                           "Vacuity",
                                           "Vredesbyrd"
                                       };

        #endregion Fields

        #region Constructors

        public PlaylistProvider()
        {
            _playlists = new List<Playlist>();
            _random = new Random();

        }

        #endregion Constructors

        #region Events

        public event EventHandler<PlaylistEventArgs> PlaylistAdded = delegate { };

        public event EventHandler<PlaylistEventArgs> PlaylistRemoved = delegate { };

        public event EventHandler<PlaylistMovedEventArgs> PlaylistMoved = delegate { };

        #endregion Events

        #region Properties

        public IEnumerable<IPlaylist> Playlists
        {
            get { return _playlists; }
        }

        #endregion Properties

        #region Public Methods

        public void Initialize()
        {
            _playlists = Builder<Playlist>
                .CreateListOfSize(_random.Next(10,25))
                .WhereAll()
                .Has(p => p.Name = _playlistNames[_random.Next(0, _playlistNames.Length - 1)])
                .Has(p => p.Tracks = GetTracks(p))
                .Build();
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<IPlaylistTrack> GetTracks(IPlaylist parentPlaylist)
        {
            return Builder<PlaylistTrack>
                .CreateListOfSize(_random.Next(6, 35))
                .WhereAll()
                .Has(t => t.Playlist = parentPlaylist)
                .Has(t => t.Name = _trackNames[_random.Next(0, _trackNames.Length - 1)])
                .Has(t => t.Duration = TimeSpan.FromSeconds(_random.Next(60, 600)))
                .Has(t => t.Album = GetAlbum())
                .Has(t => t.Artists = GetArtists())
                .Build();
        }

        private IEnumerable<ITrack> GetTracks(IAlbum album)
        {
            return Builder<Track>
                .CreateListOfSize(_random.Next(6, 35))
                .WhereAll()
                .Has(t => t.Name = _trackNames[_random.Next(0, _trackNames.Length - 1)])
                .Has(t => t.Duration = TimeSpan.FromSeconds(_random.Next(60, 600)))
                .Has(t => t.Album = album)
                .Has(t => t.Artists = GetArtists())
                .Build();
        }

        private IEnumerable<IArtist> GetArtists()
        {
            return Builder<Artist>
                .CreateListOfSize(_random.Next(10, 20))
                .WhereAll()
                .Has(a => a.Name = _artistNames[_random.Next(0, _artistNames.Length - 1)])
                .Build();
        }

        private Artist GetArtist()
        {
            return Builder<Artist>
                .CreateNew()
                .With(a => a.Name = _artistNames[_random.Next(0, _artistNames.Length - 1)])
                .Build();
        }

        private IAlbum GetAlbum()
        {
            return Builder<Album>
                .CreateNew()
                .With(a => a.Year = _random.Next(1990, 2011))
                .With(a => a.Name = _albumNames[_random.Next(0, _albumNames.Length - 1)])
                .With(a => a.Artist = GetArtist())
                .With(a => a.Tracks = GetTracks(a))
                .Build();
        }

        #endregion Private Methods
    }
}