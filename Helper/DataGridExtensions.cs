using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace ERP.Helpers
{
    public static class DataGridExtensions
    {
        public static readonly DependencyProperty BindableSelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "BindableSelectedItems",
                typeof(IList),
                typeof(DataGridExtensions),
                new PropertyMetadata(null, OnBindableSelectedItemsChanged));

        private static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid grid)
            {
                grid.SelectionChanged -= DataGrid_SelectionChanged;
                grid.SelectionChanged += DataGrid_SelectionChanged;
            }
        }

        public static void SetBindableSelectedItems(DependencyObject element, IList value)
        {
            element.SetValue(BindableSelectedItemsProperty, value);
        }

        public static IList GetBindableSelectedItems(DependencyObject element)
        {
            return (IList)element.GetValue(BindableSelectedItemsProperty);
        }

        private static void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid grid)
            {
                IList selectedItems = GetBindableSelectedItems(grid);
                if (selectedItems == null) return;

                selectedItems.Clear();
                foreach (var item in grid.SelectedItems)
                {
                    selectedItems.Add(item);
                }
            }
        }
    }
}