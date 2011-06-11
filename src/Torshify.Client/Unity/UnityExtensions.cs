using Microsoft.Practices.Unity;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Unity
{
    public static class UnityExtensions
    {
        public static void RegisterStartable<TFrom, TTo>(this IUnityContainer container)
            where TTo : IStartable, TFrom
        {
            container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
            container.RegisterType(typeof(IStartable),
                typeof(TTo),
                typeof(TTo).Name,
                new InjectionFactory(unity => unity.Resolve(typeof(TTo))));
        }

        public static IUnityContainer InstallCoreExtensions(this IUnityContainer container)
        {
            container.RemoveAllExtensions();
            container.AddExtension(new UnityClearBuildPlanStrategies());
            container.AddExtension(new UnitySafeBehaviorExtension());

#pragma warning disable 612,618 // Marked as obsolete, but Unity still uses it internally.
            container.AddExtension(new InjectedMembers());
#pragma warning restore 612,618

            container.AddExtension(new UnityDefaultStrategiesExtension());

            return container;
        }

    }
}