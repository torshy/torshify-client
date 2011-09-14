using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

using Torshify.Client.Infrastructure.Models;
using Torshify.Client.Modules.Core.Views.WhatsNew.Tabs;

namespace Torshify.Client.Modules.Core.Views.WhatsNew
{
    public class WhatsNewViewModel : TabViewModel<WhatsNewViewModel>
    {
        #region Constructors

        public WhatsNewViewModel()
        {
            AddTab(ServiceLocator.Current.TryResolve<WhatsNewTabItemView>());
            AddTab(ServiceLocator.Current.TryResolve<TopListsTabItemView>());
        }

        #endregion Constructors

        #region Methods

        protected override WhatsNewViewModel GetModel(NavigationContext navigationContext)
        {
            return this;
        }

        #endregion Methods
    }
}