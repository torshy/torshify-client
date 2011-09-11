using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;
using System.Windows.Threading;

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Services;
using Torshify.Client.Log;
using Torshify.Client.Modules.Core;
using Torshify.Client.Modules.EchoNest;
using Torshify.Client.Spotify;
using Torshify.Client.Unity;

namespace Torshify.Client
{
    public class Bootstrapper : UnityBootstrapper
    {
        #region Fields

        public static ILog BootLogger;

        #endregion Fields

        #region Methods

        public void Dispose()
        {
            Container.Dispose();
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new Log4NetFacade();
        }

        protected override void ConfigureContainer()
        {
            Container.InstallCoreExtensions();
            Container.RegisterStartable<InactivityNotificator, InactivityNotificator>();
            Container.RegisterInstance(typeof(Dispatcher), null, Application.Current.Dispatcher, new ContainerControlledLifetimeManager());
            Container.RegisterType<IBackdropService, BackdropService>(new ContainerControlledLifetimeManager(), new InjectionProperty("CacheLocation", AppConstants.BackdropCacheFolder));
            Container.RegisterType<IImageCacheService, ImageCacheService>(new ContainerControlledLifetimeManager(), new InjectionProperty("CacheLocation", AppConstants.CoverArtCacheFolder));

            base.ConfigureContainer();
        }

        protected override void ConfigureModuleCatalog()
        {
#if !MockEnabled
            Type spotifyModule = typeof(SpotifyModule);
            ModuleCatalog.AddModule(new ModuleInfo(spotifyModule.Name,
                                                   spotifyModule.AssemblyQualifiedName));

            Type coreModule = typeof(CoreModule);
            ModuleCatalog.AddModule(new ModuleInfo(coreModule.Name,
                                                   coreModule.AssemblyQualifiedName,
                                                   spotifyModule.Name));

            Type echoNestModule = typeof(EchoNestModule);
            ModuleCatalog.AddModule(new ModuleInfo(echoNestModule.Name,
                                                   echoNestModule.AssemblyQualifiedName,
                                                   coreModule.Name));
#else
            ModuleCatalog.AddModule(new ModuleInfo("Mock",
                                                   "Torshify.Client.Mocks.MockModule, Torshify.Client.Mocks"));

            Type coreModule = typeof (CoreModule);
            ModuleCatalog.AddModule(new ModuleInfo(coreModule.Name,
                                                   coreModule.AssemblyQualifiedName,
                                                   "Mock"));

            Type echoNestModule = typeof(EchoNestModule);
            ModuleCatalog.AddModule(new ModuleInfo(echoNestModule.Name,
                                                   echoNestModule.AssemblyQualifiedName,
                                                   coreModule.Name));
#endif
        }

        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<Shell>();
        }

        protected override void InitializeModules()
        {
            InitializeLogging();
            InitializeMef();
            base.InitializeModules();
            InitializeStartables();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }

        private void InitializeLogging()
        {
            var fileAppender = new RollingFileAppender();
            fileAppender.File = Path.Combine(AppConstants.LogFolder, "Torshify.log");
            fileAppender.AppendToFile = true;
            fileAppender.MaxSizeRollBackups = 10;
            fileAppender.MaxFileSize = 1024 * 1024;
            fileAppender.RollingStyle = RollingFileAppender.RollingMode.Size;
            fileAppender.StaticLogFileName = true;
            fileAppender.Layout = new PatternLayout("%date{dd MMM yyyy HH:mm} [%thread] %-5level %logger - %message%newline");
            fileAppender.Threshold = Level.Info;
            fileAppender.ActivateOptions();

            var consoleAppender = new CustomConsoleColorAppender();
            consoleAppender.AddMapping(
                new CustomConsoleColorAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity,
                    BackColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Fatal
                });
            consoleAppender.AddMapping(
                new CustomConsoleColorAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Error
                });
            consoleAppender.AddMapping(
                new CustomConsoleColorAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.Yellow | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Warn
                });
            consoleAppender.AddMapping(
                new CustomConsoleColorAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.Green | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Info
                });
            consoleAppender.AddMapping(
                new CustomConsoleColorAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Info
                });
            consoleAppender.Layout = new PatternLayout("%date{dd MM HH:mm} %-5level - %message%newline");
#if DEBUG
            consoleAppender.Threshold = Level.All;
#else
            consoleAppender.Threshold = Level.Info;
#endif
            consoleAppender.ActivateOptions();

            Logger root;
            root = ((Hierarchy)LogManager.GetRepository()).Root;
            root.AddAppender(consoleAppender);
            root.AddAppender(fileAppender);
            root.Repository.Configured = true;

            BootLogger = LogManager.GetLogger("Bootstrapper");

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Exception exception = (Exception)e.ExceptionObject;
                BootLogger.Fatal(exception);
            };

            Application.Current.Dispatcher.UnhandledException += (s, e) =>
            {
                Exception exception = e.Exception;
                BootLogger.Fatal(exception);
            };
        }

        private void InitializeMef()
        {
            AggregateCatalog aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new DirectoryCatalog(Environment.CurrentDirectory, "Torshify.*.dll"));
            CompositionContainer container = new CompositionContainer(aggregateCatalog);
            container.ComposeExportedValue(Container);
            container.ComposeExportedValue(Application.Current.Dispatcher);
            Container.RegisterInstance(container);
        }

        private void InitializeStartables()
        {
            var startables = Container.ResolveAll<IStartable>();

            foreach (var startable in startables)
            {
                startable.Start();
            }
        }

        #endregion Methods
    }
}