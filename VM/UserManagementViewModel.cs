
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
    public class UserManagementViewModel : INotifyPropertyChanged
    {
        private int CurrentPageIndex = 1;
        public ObservableCollection<User> Users { get; } = new();
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand RemoveUserCommand { get; }

        public ICommand PreviousPageCommand { get; }

        public ICommand NextPageCommand { get; }

        public ObservableCollection<User> Items { get; } = new();

        public ObservableCollection<User> SelectedCollection { get; } = new ObservableCollection<User>();


        private User? _selected;
        public User? Selected
        {
            get => _selected;
            set { _selected = value; OnPropertyChanged(); }
        }

        public UserManagementViewModel()
        {
            QuerySales();
            PreviousPageCommand = new RelayCommand(_ => previousPage());
            NextPageCommand = new RelayCommand(_ => nextPage());

            AddUserCommand = new RelayCommand(_ => { AddEditUserWindow addEditUserWindow = new AddEditUserWindow(SelectedCollection,Items); addEditUserWindow.ShowDialog(); });

            EditUserCommand = new RelayCommand(_ => { if (SelectedCollection.Count !=1) { MessageBox.Show("Select only one user, please"); return; } AddEditUserWindow addEditUserWindow = new AddEditUserWindow(SelectedCollection,Items,"Edit"); addEditUserWindow.ShowDialog(); });
            RemoveUserCommand = new RelayCommand(_ => DeleteUser());
        }

        private async void QuerySales()
        {
            var connStr = "Data Source=MYDB.db";
            using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

            var sql = "SELECT id,name,password,roles from Users LIMIT 100 OFFSET 0";

            var rows = await db.ExecuteQueryAsync(sql);

            if (rows.Any())
            {

                foreach (var row in rows)
                {
                    Items.Add(new User
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Username = row["name"]?.ToString() ?? string.Empty,
                        Password = row["password"]?.ToString() ?? string.Empty,
                        Role = row["roles"]?.ToString() ?? string.Empty
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

                var sql = $"SELECT id,name,password,roles from users LIMIT 100 OFFSET {(CurrentPageIndex - 2) * 100}";

                var rows = await db.ExecuteQueryAsync(sql);

                if (rows.Any())
                {

                    foreach (var row in rows)
                    {
                        Items.Add(new User
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Username = row["name"]?.ToString() ?? string.Empty,
                            Password = row["password"]?.ToString() ?? string.Empty,
                            Role = row["roles"]?.ToString() ?? string.Empty
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

            var sql = "SELECT count(id) from users";

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

            sql = $"SELECT id,name,password,roles from users LIMIT 100 OFFSET {(CurrentPageIndex) * 100}";

            var rows2 = await db.ExecuteQueryAsync(sql);

            if (rows2.Any())
            {

                foreach (var row in rows2)
                {
                    Items.Add(new User
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Username = row["name"]?.ToString() ?? string.Empty,
                        Password = row["password"]?.ToString() ?? string.Empty,
                        Role = row["roles"]?.ToString() ?? string.Empty
                    });
               
                }

            }


            CurrentPageIndex++;

        }

        private async void DeleteUser()
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
            var sql = $"DELETE FROM Users WHERE id IN ({string.Join(",", placeholders)});";

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


                MessageBox.Show("User deleted successfully！");

            }
            else
            {
                MessageBox.Show("Failed to delete User.");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public static readonly DependencyProperty BindableSelectedItemsProperty =
       DependencyProperty.RegisterAttached(
           "BindableSelectedItems",
           typeof(IList),
           typeof(UserManagementViewModel),
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