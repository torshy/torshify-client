namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IArtist
    {
        #region Properties

        string Name
        {
            get;
        }

        IArtistInformation Info
        {
            get;
        }

        #endregion Properties
    }
}