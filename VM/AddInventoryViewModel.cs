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
    public class AddInventoryViewModel
    {
        private string _sku;
        private string _name;
        private int _quantity;

        public string SKU { get => _sku; set { _sku = value; OnPropertyChanged(); } }
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public int Quantity { get => _quantity; set { _quantity = value; OnPropertyChanged(); } }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action Close;

        public System.Collections.ObjectModel.ObservableCollection<Models.Inventory> Inventories = new();
        public AddInventoryViewModel()
        {
            ConfirmCommand = new RelayCommand(async o =>
            {
                var connStr = "Data Source=MYDB.db"; // 替换为你的实际数据库路径
                using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

                var sql = "INSERT INTO Inventory (sku, name, amount) VALUES (@sku, @name, @amount);";

                var parameters = new List<DbParameter>
    {
        new SqliteParameter("@sku", SKU),
        new SqliteParameter("@name", Name),
        new SqliteParameter("@amount", Quantity)
    };

                var affected = await db.ExecuteNonQueryAsync(sql, parameters);
                if (affected > 0) {
                
                MessageBox.Show("Inventory added successfully！");
                    Inventories.Add(new Models.Inventory
                    {
                        SKU = SKU,
                        Name = Name,
                        Amount = Quantity.ToString()
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
