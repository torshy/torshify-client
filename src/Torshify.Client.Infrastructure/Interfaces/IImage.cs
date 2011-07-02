using System;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IImage
    {
        string ID
        {
            get;
        }

        byte[] Raw
        {
            get;
        }

        bool IsLoaded
        {
            get;
        }

        event EventHandler FinishedLoading;

        void Load();
    }
}