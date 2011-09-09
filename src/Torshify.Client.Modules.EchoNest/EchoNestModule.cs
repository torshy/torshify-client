using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Torshify.Client.Infrastructure;
using Torshify.Client.Modules.EchoNest.Views;
using Torshify.Client.Modules.EchoNest.Views.Discover;

namespace Torshify.Client.Modules.EchoNest
{
    public class EchoNestModule : IModule
    {
        #region Fields

        internal const string ApiKey = "RJOXXESVUVZ07WY1T";

        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        #endregion Fields

        #region Constructors

        public EchoNestModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        #endregion Constructors

        #region Methods

        public void Initialize()
        {
            _container.RegisterType<DiscoverView>(EchoNestViews.DiscoverMusicView);
            _container.RegisterType<EchoNestNavigationView>("EchoNestNavigationView");

            _regionManager.RegisterViewWithRegion(CoreRegionNames.MainMusicRegion, typeof(DiscoverView));

            _regionManager.RegisterViewWithRegion("Navigation", typeof(EchoNestNavigationView));
        }

        #endregion Methods
    }
}