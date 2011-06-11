using System;
using System.Collections.Generic;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class Track : ITrack
    {
        #region Properties

        public IAlbum Album
        {
            get; set;
        }

        public IEnumerable<IArtist> Artists
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
            get; set;
        }

        public int Index
        {
            get; set;
        }

        public bool IsAvailable
        {
            get; set;
        }

        public bool IsStarred
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public int Popularity
        {
            get; set;
        }

        #endregion Properties
    }
}