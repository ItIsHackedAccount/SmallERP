using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ERP.Helpers;
using ERP.Models;
using Microsoft.Data.Sqlite;

namespace ERP.ViewModels
{
   public class AddSalesViewModel
    {
        public ICommand ScrollChangedCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }

        private Inventory? _selectedSales;
        public Inventory? SelectedItem
        {
            get => _selectedSales;
            set
            {
                _selectedSales = value;
                // RaisePropertyChanged("SelectedInventory"); 如果你实现了 INotifyPropertyChanged
            }
        }

        public ObservableCollection<Inventory> Items { get; } = new ObservableCollection<Inventory>()
;
        int page = 0;

        private string _customer;
        public string Customer
        {
            get => _customer;
            set { _customer = value; }
        }

        private string _amount;
        public string Amount
        {
            get => _amount;
            set { _amount = value; }
        }

        private string _orderno;

               public string OrderNo
        {
            get => _amount;
            set { _amount = value; }
        }

        public event Action Close ;

        public System.Collections.ObjectModel.ObservableCollection<Models.Sales> Sales = new();

        public AddSalesViewModel()
        {

            ScrollChangedCommand = new RelayCommand(param => OnScrollChanged((ScrollChangedEventArgs)param));
            CancelCommand = new RelayCommand(_ => Close?.Invoke());
            ConfirmCommand= new RelayCommand(_ => addSales());

            LoadNextPage();
        }

        private async void OnScrollChanged(ScrollChangedEventArgs e)
        {
            var sv = e.Source as ScrollViewer; 
            if (sv != null)
            {
                if (sv.VerticalOffset >= sv.ScrollableHeight - 50)
                {
                    await LoadNextPage();
                }
            }


        }

        private async void addSales()
        {
            var connStr = "Data Source=MYDB.db"; // 替换为你的实际数据库路径
            using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

            var sql = "INSERT INTO Sales (orderno, customer, amount,date,inventory_id) VALUES (@orderno, @customer, @amount,@date,@inventory_id);";

            var parameters = new List<DbParameter>
    {
        new SqliteParameter("@orderno", OrderNo),
        new SqliteParameter("@customer", Customer),
        new SqliteParameter("@amount", Amount),
          new SqliteParameter("@date", DateTime.Now.ToString("G")),
        new SqliteParameter("@inventory_id", SelectedItem.Id),

    };

            var affected = await db.ExecuteNonQueryAsync(sql, parameters);
            if (affected > 0)
            {

                MessageBox.Show("Inventory added successfully！");
                Sales.Add(new Models.Sales
                {
                    OrderNo = OrderNo,
                    Customer = Customer,
                    Amount = Amount.ToString(),
                    Date = DateTime.Now.ToString("G"),
                    Inventory_ID = SelectedItem.Id
                });

            }
            else
            {

                MessageBox.Show("Failed to add inventory.");
            }
        }

        private async Task LoadNextPage()
        {

            var connStr = "Data Source=MYDB.db";
            using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

            var sql = $"SELECT id, name,sku,amount FROM Inventory LIMIT 100 OFFSET {page*100}";

            var rows = await db.ExecuteQueryAsync(sql);

            if (rows.Any())
            {

                foreach (var row in rows)
                {
                    Items.Add(new Inventory
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Name = row["name"]?.ToString() ?? string.Empty,

                    });
                }

            }
            else
            {

            }

            page++;
        }

    }
}
