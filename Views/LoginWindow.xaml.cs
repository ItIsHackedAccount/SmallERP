using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ERP.ViewModels;

namespace ERP.Views
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            var vm = new LoginViewModel();
            DataContext = vm;

            // 登录成功可以设置 DialogResult 并关闭（如果用 ShowDialog）
            vm.LoginSucceeded += () =>
            {
                // 如果窗口是以 ShowDialog() 打开的，可以设置 DialogResult
                // DialogResult = true;
                Close();
            };

            // 取消命令或 RequestClose 时关闭窗口
            vm.RequestClose += () => Close();
        }
    }
}
