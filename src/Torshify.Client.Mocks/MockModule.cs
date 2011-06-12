using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class MockModule : IModule
    {
        #region Fields

        private readonly IUnityContainer _container;

        #endregion Fields

        #region Constructors

        public MockModule(IUnityContainer container)
        {
            _container = container;
        }

        #endregion Constructors

        #region Public Methods

        public void Initialize()
        {
            _container.RegisterType<IPlaylistProvider, PlaylistProvider>(new ContainerControlledLifetimeManager(), new InjectionMethod("Initialize"));
            _container.RegisterType<IPlayer, Player>(new ContainerControlledLifetimeManager());
        }

        #endregion Public Methods
    }
}