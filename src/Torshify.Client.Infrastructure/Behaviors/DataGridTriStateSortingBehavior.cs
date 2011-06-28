using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Torshify.Client.Infrastructure.Behaviors
{
    public class DataGridTriStateSortingBehavior : Behavior<DataGrid>
    {
        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Sorting += TriStateSorting;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Sorting -= TriStateSorting;
        }

        private void TriStateSorting(object sender, DataGridSortingEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            string sortPropertyName = Helpers.GetSortMemberPath(e.Column);

            if (!string.IsNullOrEmpty(sortPropertyName))
            {
                // sorting is cleared when the previous state is Descending

                if (e.Column.SortDirection.HasValue && e.Column.SortDirection.Value == ListSortDirection.Descending)
                {
                    int index = Helpers.FindSortDescription(dataGrid.Items.SortDescriptions, sortPropertyName);

                    if (index != -1)
                    {
                        e.Column.SortDirection = null;

                        // remove the sort description
                        dataGrid.Items.SortDescriptions.RemoveAt(index);
                        dataGrid.Items.Refresh();

                        if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                        {
                            // clear any other sort descriptions for the multisorting case
                            dataGrid.Items.SortDescriptions.Clear();
                            dataGrid.Items.Refresh();
                        }

                        // stop the default sort
                        e.Handled = true;
                    }
                }
            }
        }

        #endregion Methods

        #region Nested Types

        public static class Helpers
        {
            #region Methods

            public static int FindSortDescription(SortDescriptionCollection sortDescriptions, string sortPropertyName)
            {
                int index = -1;
                int i = 0;
                foreach (SortDescription sortDesc in sortDescriptions)
                {
                    if (string.Compare(sortDesc.PropertyName, sortPropertyName) == 0)
                    {
                        index = i;
                        break;
                    }
                    i++;
                }

                return index;
            }

            public static string GetSortMemberPath(DataGridColumn column)
            {
                // find the sortmemberpath
                string sortPropertyName = column.SortMemberPath;
                if (string.IsNullOrEmpty(sortPropertyName))
                {
                    DataGridBoundColumn boundColumn = column as DataGridBoundColumn;
                    if (boundColumn != null)
                    {
                        Binding binding = boundColumn.Binding as Binding;
                        if (binding != null)
                        {
                            if (!string.IsNullOrEmpty(binding.XPath))
                            {
                                sortPropertyName = binding.XPath;
                            }
                            else if (binding.Path != null)
                            {
                                sortPropertyName = binding.Path.Path;
                            }
                        }
                    }
                }

                return sortPropertyName;
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}