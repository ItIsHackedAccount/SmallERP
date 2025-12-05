using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ERP.Helper;
using ERP.Helpers;
using ERP.Models;
using Microsoft.Data.Sqlite;
namespace ERP.ViewModels
{
    public class DispatchReportViewModel : NotifyBase
    {
        private int CurrentPageIndex = 1;
        public ObservableCollection<Dispatch> Items { get; } = new();
        public ObservableCollection<Dispatch> SelectedCollection { get; } = new ObservableCollection<Dispatch>();
        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }

        public ICommand PreviousPageCommand { get; }

        public ICommand NextPageCommand { get; }

        private Inventory? _selected;
        public Inventory? Selected
        {
            get => _selected;
            set { _selected = value; OnPropertyChanged(); }
        }

        public DispatchReportViewModel()
        {
            AddItemCommand = new RelayCommand(_ => { });
            RemoveItemCommand = new RelayCommand(_ => DeleteDispatch());
            PreviousPageCommand = new RelayCommand(_ => previousPage());
            NextPageCommand = new RelayCommand(_ => nextPage());
            QueryDispatch();
        }

        private async void QueryDispatch()
        {
            var connStr = "Data Source=MYDB.db";
            using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

            var sql = "SELECT d.id,   d.DispatchNo,   d.OrderId,  d.CustomerId,   d.date,    d.status,    c.name,    s.OrderNo FROM Dispatch AS d INNER JOIN Customer AS c ON c.id = d.CustomerId INNER JOIN Sales AS s ON s.id = d.OrderId LIMIT 100 OFFSET 0";

            var rows = await db.ExecuteQueryAsync(sql);

            if (rows.Any())
            {

                foreach (var row in rows)
                {
                    Items.Add(new Dispatch
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Customer = row["name"]?.ToString() ?? string.Empty,
                        DispatchNo = row["DispatchNo"]?.ToString() ?? string.Empty,
                        OrderNo = row["OrderNo"]?.ToString() ?? string.Empty
                    });
                }

            }
            else
            {

            }
        }

        private async void nextPage()
        {
            int totalPages = 0;
            var connStr = "Data Source=MYDB.db";
            using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

            var sql = "SELECT count(d.id) FROM Dispatch AS d INNER JOIN Customer AS c ON c.id = d.CustomerId INNER JOIN Sales AS s ON s.id = d.OrderId";



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

            sql = $"SELECT d.id,   d.DispatchNo,   d.OrderId,  d.CustomerId,   d.date,    d.status,    c.name,    s.OrderNo FROM Dispatch AS d INNER JOIN Customer AS c ON c.id = d.CustomerId INNER JOIN Sales AS s ON s.id = d.OrderId LIMIT 100 OFFSET {(CurrentPageIndex) * 100}";

            var rows2 = await db.ExecuteQueryAsync(sql);

            if (rows2.Any())
            {

                foreach (var row in rows2)
                {
                    Items.Add(new Dispatch
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Customer = row["name"]?.ToString() ?? string.Empty,
                        DispatchNo = row["DispatchNo"]?.ToString() ?? string.Empty,
                        OrderNo = row["OrderNo"]?.ToString() ?? string.Empty
                    });
                }

            }


            CurrentPageIndex++;

        }

        private async void previousPage()
        {
            if (CurrentPageIndex > 1)
            {
                var connStr = "Data Source=MYDB.db";
                using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

                var sql = $"SELECT d.id,   d.DispatchNo,   d.OrderId,  d.CustomerId,   d.date,    d.status,    c.name,    s.OrderNo FROM Dispatch AS d INNER JOIN Customer AS c ON c.id = d.CustomerId INNER JOIN Sales AS s ON s.id = d.OrderId LIMIT 100 OFFSET {(CurrentPageIndex - 2) * 100}";

                var rows = await db.ExecuteQueryAsync(sql);

                if (rows.Any())
                {

                    foreach (var row in rows)
                    {
                        Items.Add(new Dispatch
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Customer = row["name"]?.ToString() ?? string.Empty,
                            DispatchNo = row["DispatchNo"]?.ToString() ?? string.Empty,
                            OrderNo = row["OrderNo"]?.ToString() ?? string.Empty
                        });
                    }
                }
                CurrentPageIndex--;
            }

        }


        private async void DeleteDispatch()
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
            var sql = $"DELETE FROM Dispatch WHERE id IN ({string.Join(",", placeholders)});";

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


                MessageBox.Show("Dispatch deleted successfully！");

            }
            else
            {
                MessageBox.Show("Failed to delete Dispatch.");
            }
        }
    }
}
