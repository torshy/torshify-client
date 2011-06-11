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

        #endregion Properties
    }
}