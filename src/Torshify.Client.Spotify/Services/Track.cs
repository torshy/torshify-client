using System;
using System.Collections.Generic;

using Microsoft.Practices.Prism.ViewModel;

using ITorshifyAlbum = Torshify.Client.Infrastructure.Interfaces.IAlbum;

using ITorshifyArtist = Torshify.Client.Infrastructure.Interfaces.IArtist;

using ITorshifyTrack = Torshify.Client.Infrastructure.Interfaces.ITrack;

namespace Torshify.Client.Spotify.Services
{
    public class Track : NotificationObject, ITorshifyTrack
    {
        #region Fields

        private int _id;
        private int _index;

        #endregion Fields

        #region Constructors

        protected Track(ITrack track)
        {
            InternalTrack = track;
        }

        #endregion Constructors

        #region Properties

        public ITorshifyAlbum Album
        {
            get; set;
        }

        public IEnumerable<ITorshifyArtist> Artists
        {
            get; set;
        }

        public int Disc
        {
            get; set;
        }

        public TimeSpan Duration
        {
            get; set;
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public bool IsAvailable
        {
            get; private set;
        }

        public bool IsStarred
        {
            get; set;
        }

        public string Name
        {
            get { return InternalTrack.Name; }
        }

        public int Popularity
        {
            get; private set;
        }

        public ITrack InternalTrack
        {
            get; private set;
        }

        #endregion Properties
    }
}