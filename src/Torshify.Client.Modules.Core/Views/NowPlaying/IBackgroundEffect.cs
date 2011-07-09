using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.NowPlaying
{
    public interface IBackgroundEffect
    {
        #region Methods

        void NavigatedFrom();

        void NavigatedTo();

        void OnTrackChanged(ITrack previous, ITrack current);

        #endregion Methods
    }
}