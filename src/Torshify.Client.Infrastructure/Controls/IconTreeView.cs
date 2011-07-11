using System.Windows;
using System.Windows.Controls;

namespace Torshify.Client.Infrastructure.Controls
{
    public class IconTreeView : TreeView
    {
        public static readonly DependencyProperty IconTemplateSelectorProperty =
            DependencyProperty.Register("IconTemplateSelector", typeof(DataTemplateSelector), typeof(IconTreeView),
                new FrameworkPropertyMetadata((DataTemplateSelector)null));

        public DataTemplateSelector IconTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(IconTemplateSelectorProperty); }
            set { SetValue(IconTemplateSelectorProperty, value); }
        }

        static IconTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconTreeView), new FrameworkPropertyMetadata(typeof(IconTreeView)));
        }
    }
}