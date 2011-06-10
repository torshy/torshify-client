using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

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
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;
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

        #region Private Methods

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            File.WriteAllText("CrashOnDispatcher.log", e.Exception.ToString());
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            File.WriteAllText("Crash.log", ex.ToString());
        }

        #endregion Private Methods
    }
}