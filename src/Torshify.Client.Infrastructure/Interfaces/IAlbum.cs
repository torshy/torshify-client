namespace Torshify.Client.Infrastructure.Interfaces
{
    public enum AlbumType
    {
        Album,
        Single,
        Compilation,
        Unknown
    }

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

        IImage CoverArt
        {
            get;
        }

        IAlbumInformation Info
        {
            get;
        }

        AlbumType Type
        {
            get;
        }

        #endregion Properties
    }
}