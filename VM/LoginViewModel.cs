using System;
using System.ComponentModel;
using System.Data.Common;
using System.Windows;
using System.Windows.Input;
using ERP.Helpers;
using Microsoft.Data.Sqlite;

namespace ERP.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _errorMessage;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Username)); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        public ICommand LoginCommand { get; }

        public ICommand CancelCommand { get; }

        // 登录成功事件，供视图订阅
        public event Action LoginSucceeded;

        // 请求关闭窗口（由视图订阅）
        public event Action RequestClose;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
            CancelCommand = new RelayCommand(_=>LogOut());
        }

        private async void ExecuteLogin(object parameter)
        {

            if(string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Please enter username and password.");
                return;
            }
            var connStr = "Data Source=MYDB.db"; // 替换为你的实际数据库路径
            using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

            var sql = "SELECT id, name FROM Users WHERE name = @name AND password = @password";

            var parameters = new List<DbParameter>
{
    new SqliteParameter("@name", Username),
    new SqliteParameter("@password", Password)
};


            var rows = await db.ExecuteQueryAsync(sql, parameters);

            if (rows.Any())
            {
                ErrorMessage = string.Empty;

                MainWindow m = new MainWindow(Username);
                m.Show();
                LoginSucceeded?.Invoke();


            }
            else
            {
                ErrorMessage = "Wrong username or password";
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void LogOut()
        {
          
            RequestClose?.Invoke();

        }
    }

}