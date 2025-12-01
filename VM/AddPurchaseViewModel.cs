using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ERP.Helpers;
using Microsoft.Data.Sqlite;

namespace ERP.ViewModels
{
    internal class AddPurchaseViewModel
    {

        private string _orderno;
        private string _vendor;
        private int _amount;

        private string _productname;

        public string OrderNo { get => _orderno; set { _orderno = value; OnPropertyChanged(); } }
        public string Vendor { get => _vendor; set { _vendor = value; OnPropertyChanged(); } }
        public int Amount { get => _amount; set { _amount = value; OnPropertyChanged(); } }

        public string ProductName { get => _productname; set { _productname = value; OnPropertyChanged(); } }

        public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        public event Action Close;

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public System.Collections.ObjectModel.ObservableCollection<Models.Purchase> Purchases = new();






        public AddPurchaseViewModel()
        {
            ConfirmCommand = new RelayCommand(async o =>
            {
                var connStr = "Data Source=MYDB.db"; // 替换为你的实际数据库路径
                using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

                var sql = "INSERT INTO Purchase (orderno, vendor, amount,date,productname) VALUES (@orderno, @vendor, @amount,@date,@productname);";

                var parameters = new List<DbParameter>
    {
        new SqliteParameter("@orderno", OrderNo),
        new SqliteParameter("@vendor", Vendor),
        new SqliteParameter("@amount", Amount),
          new SqliteParameter("@date", DateTime.Now.ToString("G")),
        new SqliteParameter("@productname", ProductName),

    };

                var affected = await db.ExecuteNonQueryAsync(sql, parameters);
                if (affected > 0)
                {

                    MessageBox.Show("Inventory added successfully！");
                    Purchases.Add(new Models.Purchase
                    {
                        OrderNo = OrderNo,
                        Vendor = Vendor,
                        Amount = Amount.ToString(),
                        Date= DateTime.Now.ToString("G"),
                        ProductName=ProductName
                    });
  
                }
                else
                {

                    MessageBox.Show("Failed to add inventory.");
                }
            });


            CancelCommand = new RelayCommand(o =>
            {
                Close?.Invoke();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
