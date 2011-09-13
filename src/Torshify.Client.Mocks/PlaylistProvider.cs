using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Torshify.Client.Infrastructure.Collections;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class PlaylistProvider : IPlaylistProvider, IInitializable
    {
        #region Fields

        private IList<Playlist> _playlists;
        private Random _random;

        private string _latinText =
            "Eu sed putant vituperata efficiantur, prompta repudiandae at per. Novum integre equidem vix et, sea an quidam noster, eum nulla propriae ad. Justo mollis volutpat ius at, cum nihil tacimates at. Modo cibo decore sit et.";

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

        public void Move(int oldIndex, int newIndex)
        {

        }

        public void Remove(int index)
        {

        }

        public void Initialize()
        {
            _playlists = Builder<Playlist>
                .CreateListOfSize(_random.Next(10, 25))
                .WhereAll()
                .Has(p => p.Name = _playlistNames[_random.Next(0, _playlistNames.Length - 1)])
                .Has(p => p.Tracks = GetTracks(p))
                .Build();
        }

        #endregion Public Methods

        #region Private Methods

        private INotifyEnumerable<IPlaylistTrack> GetTracks(IPlaylist parentPlaylist)
        {
            return new NotifyCollection<IPlaylistTrack>(Builder<PlaylistTrack>
                .CreateListOfSize(_random.Next(6, 35))
                .WhereAll()
                .Has(t => t.Playlist = parentPlaylist)
                .Has(t => t.Name = _trackNames[_random.Next(0, _trackNames.Length - 1)])
                .Has(t => t.Duration = TimeSpan.FromSeconds(_random.Next(60, 600)))
                .Has(t => t.Album = GetAlbum())
                .Has(t => t.Artists = GetArtists())
                .WhereRandom(6)
                .Has(t => ((Album)t.Album).IsAvailable = false)
                .Has(t => t.IsAvailable = false)
                .Build());
        }

        private INotifyEnumerable<ITrack> GetTracks(IAlbum album)
        {
            return new NotifyCollection<Track>(Builder<Track>
                                                   .CreateListOfSize(_random.Next(6, 35))
                                                   .WhereAll()
                                                   .Has(
                                                       t =>
                                                       t.Name = _trackNames[_random.Next(0, _trackNames.Length - 1)])
                                                   .Has(t => t.Duration = TimeSpan.FromSeconds(_random.Next(60, 600)))
                                                   .Has(t => t.Album = album)
                                                   .Has(t => t.Artists = GetArtists())
                                                   .WhereRandom(6)
                                                   .Has(t => ((Album)t.Album).IsAvailable = false)
                                                   .Has(t => t.IsAvailable = false)
                                                   .Build());
        }

        private INotifyEnumerable<IArtist> GetArtists()
        {
            return new NotifyCollection<Artist>(Builder<Artist>
                                                    .CreateListOfSize(_random.Next(10, 20))
                                                    .WhereAll()
                                                    .Has(
                                                        a =>
                                                        a.Name = _artistNames[_random.Next(0, _artistNames.Length - 1)])
                                                    .Build());
        }

        private Artist GetArtist()
        {
            return Builder<Artist>
                .CreateNew()
                .With(a => a.Name = _artistNames[_random.Next(0, _artistNames.Length - 1)])
                .With(a => a.Info = GetArtistInformation(a))
                .Build();
        }

        private IArtistInformation GetArtistInformation(IArtist artist)
        {
            return Builder<ArtistInformation>
                .CreateNew()
                .With(i => i.Albums = GetAlbums(artist))
                .With(i => i.Biography = _latinText)
                .Build();
        }

        private INotifyEnumerable<IAlbum> GetAlbums(IArtist artist)
        {
            return new NotifyCollection<IAlbum>(Builder<Album>
                .CreateListOfSize(_random.Next(3, 5))
                .WhereAll()
                .Has(a => a.Year = _random.Next(1990, 2011))
                .Has(a => a.Name = _albumNames[_random.Next(0, _albumNames.Length - 1)])
                .Has(a => a.Artist = artist)
                .Has(a => a.Info = GetAlbumInformation(a))
                .Has(a => a.IsAvailable = true)
                .Build());
        }

        private IAlbum GetAlbum()
        {
            return Builder<Album>
                .CreateNew()
                .With(a => a.Year = _random.Next(1990, 2011))
                .With(a => a.Name = _albumNames[_random.Next(0, _albumNames.Length - 1)])
                .With(a => a.Artist = GetArtist())
                .With(a => a.Info = GetAlbumInformation(a))
                .With(a => a.IsAvailable = true)
                .Build();
        }

        private IAlbumInformation GetAlbumInformation(IAlbum album)
        {
            return Builder<AlbumInformation>
                .CreateNew()
                .With(i => i.Review = _latinText)
                .With(i => i.Tracks = GetTracks(album))
                .With(i => i.Copyrights = GetCopyrights())
                .Build();
        }

        private INotifyEnumerable<string> GetCopyrights()
        {
            NotifyCollection<string> copyrights = new NotifyCollection<string>();
            copyrights.Add("2011 Torshify Records");
            copyrights.Add("2010 Mock Records");
            return copyrights;
        }

        #endregion Private Methods
    }
}