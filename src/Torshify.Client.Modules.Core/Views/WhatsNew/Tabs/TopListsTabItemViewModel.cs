using System;
using System.Windows;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.WhatsNew.Tabs
{
    public class TopListsTabItemViewModel : NotificationObject, ITabViewModel<object>
    {
        #region Properties

        public string Header
        {
            get { return "Top Lists"; }
        }

        public Visibility Visibility
        {
            get { return Visibility.Visible; }
        }

        #endregion Properties

        #region Methods

        public void Deinitialize(NavigationContext navContext)
        {
        }

        public void Initialize(NavigationContext navContext)
        {
        }

        public void SetModel(object model)
        {
        }

        public void NavigatedTo()
        {
        }

        public void NavigatedFrom()
        {
        }

        #endregion Methods
    }
}