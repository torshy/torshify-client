using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Torshify.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            base.OnStartup(e);

            Bootstrapper = new Bootstrapper();
            Bootstrapper.Run();
        }

        protected Bootstrapper Bootstrapper { get; set; }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Bootstrapper != null)
            {
                Bootstrapper.Dispose();
                Bootstrapper = null;
            }

            base.OnExit(e);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            File.WriteAllText("CrashOnDispatcher.log", e.Exception.ToString());
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            File.WriteAllText("Crash.log", ex.ToString());
        }
    }
}
