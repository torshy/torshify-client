using System.Windows;

using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;

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

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }

        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<Shell>();
        }

        #endregion Protected Methods
    }
}