using System.Windows;
using System.Windows.Controls;

namespace Torshify.Client.Infrastructure.Models
{
    public class ResourcedNavigationItemTemplateSelector : DataTemplateSelector
    {
        #region Properties

        public ResourceDictionary Templates
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            NavigationItem navigationItem = item as NavigationItem;

            if (navigationItem != null)
            {
                if (Templates.Contains(navigationItem.NavigationUrl.OriginalString))
                {
                    return Templates[navigationItem.NavigationUrl.OriginalString] as DataTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }

        #endregion Methods
    }
}