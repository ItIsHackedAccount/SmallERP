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
using ERP.Models;
using Microsoft.Data.Sqlite;

namespace ERP.ViewModels
{
    public class AddEditUserViewModel
    {
        public event Action Close;

        public System.Collections.ObjectModel.ObservableCollection<Models.User> Users = new();

        public System.Collections.ObjectModel.ObservableCollection<Models.User> SelectedItem = new();
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public string tag = string.Empty;

        private string _name;
        private string _password;
        private string _role;

        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string Role { get => _role; set { _role = value; OnPropertyChanged(); } }

        public AddEditUserViewModel()
        {
            ConfirmCommand = new Helpers.RelayCommand(async o =>
            {
                // Here you would add code to save the new user to the database
                // For demonstration, we'll just add a dummy user
                if (tag.Equals("Edit"))
                {
                    int id = 0;
                    foreach (Models.User u in SelectedItem)
                    {

                        id = u.Id;

                    }
                    var connStr = "Data Source=MYDB.db"; // 替换为你的实际数据库路径
                    using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

                    var sql = "update  Users set name = @name,password= @password,roles= @roles where id= @id";

                    var parameters = new List<DbParameter>
    {
        new SqliteParameter("@name", Name),
        new SqliteParameter("@password", Password),
        new SqliteParameter("@roles", Role),
        new SqliteParameter("@id", id)
    };

                    var affected = await db.ExecuteNonQueryAsync(sql, parameters);
                    if (affected > 0)
                    {

                        MessageBox.Show("User edited successfully！");
                        Users.Where(x => x.Id == id).ToList().ForEach(u =>
                        {
                            u.Username = Name;
                            u.Password = Password;
                            u.Role = Role;
                        });

                    }
                    else
                    {

                        MessageBox.Show("Failed to edit User.");
                    }

                }

                else
                {


                    var connStr = "Data Source=MYDB.db"; // 替换为你的实际数据库路径
                    using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

                    var sql = "INSERT INTO Users (name, password, roles) VALUES (@name, @password, @roles);";

                    var parameters = new List<DbParameter>
    {
        new SqliteParameter("@name", Name),
        new SqliteParameter("@password", Password),
        new SqliteParameter("@roles", Role)
    };

                    var affected = await db.ExecuteNonQueryAsync(sql, parameters);
                    if (affected > 0)
                    {

                        MessageBox.Show("User added successfully！");
                        Users.Add(new Models.User
                        {

                            Username = Name,
                            Password = Password,
                            Role = Role
                        });
                    }
                    else
                    {

                        MessageBox.Show("Failed to add User.");
                    }
                }

            });
            CancelCommand = new Helpers.RelayCommand(o =>
            {
                Close?.Invoke();
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
