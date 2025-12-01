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

namespace ERP.Views
{
    /// <summary>
    /// AddPurchaseWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddPurchaseWindow : Window
    {
        public AddPurchaseWindow(System.Collections.ObjectModel.ObservableCollection<Models.Purchase> items)
        {
            InitializeComponent();
            if (this.DataContext is ViewModels.AddPurchaseViewModel vm)
            {
                vm.Close += () => this.Close();
                vm.Purchases = items;
            }
        }
    }
}
