using System.Threading;
using Microsoft.Practices.Unity;

namespace Torshify.Client.Unity
{
    /// <summary>
    /// Replaces <see cref="UnityDefaultBehaviorExtension"/> to eliminate 
    /// <see cref="SynchronizationLockException"/> exceptions that would otherwise occur
    /// when using <c>RegisterInstance</c>.
    /// </summary>
    public class UnitySafeBehaviorExtension : UnityDefaultBehaviorExtension
    {
        /// <summary>
        /// Adds this extension's behavior to the container.
        /// </summary>
        protected override void Initialize()
        {
            Context.RegisteringInstance += PreRegisteringInstance;

            base.Initialize();
        }

        /// <summary>
        /// Handles the <see cref="ExtensionContext.RegisteringInstance"/> event by
        /// ensuring that, if the lifetime manager is a 
        /// <see cref="SynchronizedLifetimeManager"/> that its 
        /// <see cref="SynchronizedLifetimeManager.GetValue"/> method has been called.
        /// </summary>
        /// <param name="sender">The object responsible for raising the event.</param>
        /// <param name="e">A <see cref="RegisterInstanceEventArgs"/> containing the
        /// event's data.</param>
        private void PreRegisteringInstance(object sender, RegisterInstanceEventArgs e)
        {
            if (e.LifetimeManager is SynchronizedLifetimeManager)
            {
                e.LifetimeManager.GetValue();
            }
        }
    }

}