using System;
using System.Collections.Generic;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface ITrack
    {
        #region Properties

        IAlbum Album
        {
            get;
        }

        IEnumerable<IArtist> Artists
        {
            get;
        }

        int Disc
        {
            get;
        }

        TimeSpan Duration
        {
            get;
        }

        int ID
        {
            get;
        }

        int Index
        {
            get;
        }

        bool IsAvailable
        {
            get;
        }

        bool IsStarred
        {
            get; set;
        }

        string Name
        {
            get;
        }

        int Popularity
        {
            get;
        }

        #endregion Properties
    }
}