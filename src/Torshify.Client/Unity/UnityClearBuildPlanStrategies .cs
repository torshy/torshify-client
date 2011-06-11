using Microsoft.Practices.Unity;

namespace Torshify.Client.Unity
{
    /// <summary>
    /// Implements a <see cref="UnityContainerExtension"/> that clears the list of 
    /// build plan strategies held by the container.
    /// </summary>
    public class UnityClearBuildPlanStrategies : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Context.BuildPlanStrategies.Clear();
        }
    }

}