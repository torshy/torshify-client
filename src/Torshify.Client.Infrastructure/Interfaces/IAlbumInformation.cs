using System;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IAlbumInformation
    {
        #region Events

        event EventHandler<AlbumInformationEventArgs> Loaded;

        #endregion Events

        #region Properties

        INotifyEnumerable<string> Copyrights
        {
            get;
        }

        bool IsLoading
        {
            get;
        }

        string Review
        {
            get;
        }

        INotifyEnumerable<ITrack> Tracks
        {
            get;
        }

        #endregion Properties
    }

    public class AlbumInformationEventArgs : EventArgs
    {
        #region Constructors

        public AlbumInformationEventArgs(IAlbum album)
        {
            Album = album;
        }

        #endregion Constructors

        #region Properties

        public IAlbum Album
        {
            get; set;
        }

        #endregion Properties
    }
}