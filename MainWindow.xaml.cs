using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ERP.ViewModels;

namespace ERP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string username;


        public MainWindow(string username)
        {
            InitializeComponent();
            this.username = username;
        }



        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.UserName = username;
                vm.RequestClose += () => this.Close();
                vm.RequestMaximize += () => {
                    double height = SystemParameters.WorkArea.Height;
                    double width = SystemParameters.WorkArea.Width;
                    this.Height = height;
                    this.Width = width;
                    this.Left = SystemParameters.WorkArea.Left;
                    this.Top = SystemParameters.WorkArea.Top;
                
                };
                vm.RequestMinimize += () => this.WindowState = WindowState.Minimized;
            }
            double height = SystemParameters.WorkArea.Height;
            double width = SystemParameters.WorkArea.Width;
            this.Height = height;
            this.Width = width;
            this.Left = SystemParameters.WorkArea.Left;
            this.Top = SystemParameters.WorkArea.Top;




        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // 双击：切换最大化/还原
                if (this.WindowState == WindowState.Normal)
                {
                    double height = SystemParameters.WorkArea.Height;
                    double width = SystemParameters.WorkArea.Width;
                    this.Height = height;
                    this.Width = width;
                    this.Left = SystemParameters.WorkArea.Left;
                    this.Top = SystemParameters.WorkArea.Top;


                }
                else
                {
                    this.WindowState = WindowState.Normal;
                }
            }
            else if (e.ButtonState == MouseButtonState.Pressed)
            {
                // 单击：拖动窗口
                this.DragMove();
            }

        }


    }
}