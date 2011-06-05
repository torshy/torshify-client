using System;
using System.Windows;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Modules.Core;

namespace Torshify.Client
{
    public class Bootstrapper : UnityBootstrapper
    {
        #region Public Methods

        public void Dispose()
        {
            Container.Dispose();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void ConfigureContainer()
        {
            Container.RegisterStartable<InactivityNotificator, InactivityNotificator>();
            Container.RegisterInstance(typeof(Dispatcher), null, Application.Current.Dispatcher, new ContainerControlledLifetimeManager());
            base.ConfigureContainer();
        }

        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<Shell>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();
            InitializeStartables();
        }

        protected override void ConfigureModuleCatalog()
        {
            Type coreModule = typeof (CoreModule);
            ModuleCatalog.AddModule(new ModuleInfo(coreModule.Name,
                                                   coreModule.AssemblyQualifiedName));
        }

        #endregion Protected Methods

        #region Private Methods

        private void InitializeStartables()
        {
            var startables = Container.ResolveAll<IStartable>();

            foreach (var startable in startables)
            {
                startable.Start();
            }
        }

        #endregion Private Methods
    }
}