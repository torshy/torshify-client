using System.Collections;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IAlbum
    {
        #region Properties

        IArtist Artist
        {
            get;
        }

        bool IsAvailable
        {
            get;
        }

        string Name
        {
            get;
        }

        int Year
        {
            get;
        }

        BitmapSource Cover
        {
            get;
        }

        IEnumerable<ITrack> Tracks
        {
            get;
        }

        #endregion Properties
    }
}