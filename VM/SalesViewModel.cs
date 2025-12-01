
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ERP.Helpers;
using ERP.Models;
using ERP.Views;
using Microsoft.Data.Sqlite;

namespace ERP.ViewModels
{
    public class SalesViewModel : INotifyPropertyChanged
    {
        private int CurrentPageIndex = 1;
        public ObservableCollection<Order> Orders { get; } = new();
        public ICommand AddOrderCommand { get; }
        public ICommand RemoveOrderCommand { get; }

        public ICommand PreviousPageCommand { get; }

        public ICommand NextPageCommand { get; }

        public ObservableCollection<Sales> Items { get; } = new();

        public ObservableCollection<Sales> SelectedCollection { get; } = new ObservableCollection<Sales>();

        private Order? _selected;
        public Order? Selected
        {
            get => _selected;
            set { _selected = value; OnPropertyChanged(); }
        }

        public SalesViewModel()
        {
            PreviousPageCommand = new RelayCommand(_ => previousPage());
            NextPageCommand = new RelayCommand(_ => nextPage());
            QuerySales();
            Orders.Add(new Order { Customer = "客户甲", Total = 123.45m });
            AddOrderCommand = new RelayCommand(_ => { AddSalesWindow addSalesWindow = new AddSalesWindow(Items); addSalesWindow.ShowDialog(); });
            RemoveOrderCommand = new RelayCommand(_ => DeleteSales());
       
        }

        private async void QuerySales()
        {
            var connStr = "Data Source=MYDB.db";
            using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

            var sql = "SELECT a.id, a.orderno,a.customer,a.amount,b.name FROM Sales as a inner join Inventory as b on a.inventory_id = b.id LIMIT 100 OFFSET 0";

            var rows = await db.ExecuteQueryAsync(sql);

            if (rows.Any())
            {

                foreach (var row in rows)
                {
                    Items.Add(new Sales
                    {
                        Id = Convert.ToInt32(row["id"]),
                        OrderNo = row["orderno"]?.ToString() ?? string.Empty,
                        Customer = row["customer"]?.ToString() ?? string.Empty,
                        Amount = row["amount"]?.ToString() ?? string.Empty
                    });
                }

            }
            else
            {

            }

        }

        private async void previousPage()
        {
            if (CurrentPageIndex > 1)
            {
                var connStr = "Data Source=MYDB.db";
                using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

                var sql = $"SELECT a.id, a.orderno,a.customer,a.amount,b.name FROM Sales as a inner join Inventory as b on a.inventory_id = b.id LIMIT 100 OFFSET {(CurrentPageIndex - 2) * 100}";

                var rows = await db.ExecuteQueryAsync(sql);

                if (rows.Any())
                {

                    foreach (var row in rows)
                    {
                        Items.Add(new Sales
                        {
                            Id = Convert.ToInt32(row["id"]),
                            OrderNo = row["orderno"]?.ToString() ?? string.Empty,
                            Customer = row["customer"]?.ToString() ?? string.Empty,
                            Amount = row["amount"]?.ToString() ?? string.Empty
                        });
                    }

                }

                CurrentPageIndex--;

            }
        }

        private async void nextPage()
        {
            int totalPages = 0;
            var connStr = "Data Source=MYDB.db";
            using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

            var sql = "SELECT count(a.id) FROM Sales as a inner join Inventory as b on a.inventory_id = b.id";

            //var connStr = "Data Source=MYDB.db";
            //using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

            //var sql = "SELECT id, name,sku,amount FROM Inventory LIMIT 100 OFFSET 0";

            var rows = await db.ExecuteScalarAsync(sql);

            if (Convert.ToInt16(rows) < 0)
            {
                return;

            }
            int totalRows = Convert.ToInt16(rows);
            int modulo = totalRows % 100;

            if (modulo > 0)
            {
                totalPages = totalRows / 100 + 1;

            }
            else
            {
                totalPages = totalRows / 100;
            }

            if (CurrentPageIndex >= totalPages)
            {
                return;
            }

            sql = $"SELECT a.id, a.orderno,a.customer,a.amount,b.name FROM Sales as a inner join Inventory as b on a.inventory_id = b.id LIMIT 100 OFFSET {(CurrentPageIndex) * 100}";

            var rows2 = await db.ExecuteQueryAsync(sql);

            if (rows2.Any())
            {

                foreach (var row in rows2)
                {
                    Items.Add(new Sales
                    {
                        Id = Convert.ToInt32(row["id"]),
                        OrderNo = row["orderno"]?.ToString() ?? string.Empty,
                        Customer = row["customer"]?.ToString() ?? string.Empty,
                        Amount = row["amount"]?.ToString() ?? string.Empty
                    });
                }

            }


            CurrentPageIndex++;

        }

        private async void DeleteSales()
        {

            var ids = SelectedCollection.Select(x => x.Id).ToList();


            var parameters = new List<DbParameter>();
            var placeholders = new List<string>();

            for (int i = 0; i < ids.Count; i++)
            {
                string paramName = $"@id{i}";
                placeholders.Add(paramName);
                parameters.Add(new SqliteParameter(paramName, ids[i]));
            }

            // 3. 拼接 SQL
            var sql = $"DELETE FROM Sales WHERE id IN ({string.Join(",", placeholders)});";

            // 4. 执行
            var connStr = "Data Source=MYDB.db";
            using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);
            var affected = await db.ExecuteNonQueryAsync(sql, parameters);
            if (affected > 0)
            {

                foreach (var selectedItem in SelectedCollection.ToList())
                {
                    Items.Remove(selectedItem);
                }


                MessageBox.Show("Sales deleted successfully！");

            }
            else
            {
                MessageBox.Show("Failed to delete Sales.");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public static readonly DependencyProperty BindableSelectedItemsProperty =
         DependencyProperty.RegisterAttached(
             "BindableSelectedItems",
             typeof(IList),
             typeof(SalesViewModel),
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