using System.Windows;

using Torshify.Client.Spotify;

namespace Torshify.Client
{
    public partial class App : Application
    {
        #region Constructors

        public App()
        {
            SpotifyModule.InitializeLibspotify();
        }

        #endregion Constructors

        #region Properties

        protected Bootstrapper Bootstrapper
        {
            get;
            private set;
        }

        #endregion Properties

        #region Protected Methods

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Bootstrapper = new Bootstrapper();
            Bootstrapper.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Bootstrapper != null)
            {
                Bootstrapper.Dispose();
                Bootstrapper = null;
            }

            base.OnExit(e);
        }

        #endregion Protected Methods
    }
}