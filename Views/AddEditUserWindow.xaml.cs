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
    /// AddEditUserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddEditUserWindow : Window
    {
        public AddEditUserWindow(System.Collections.ObjectModel.ObservableCollection<Models.User> SelectedCollection, System.Collections.ObjectModel.ObservableCollection<Models.User> items, string tag="")
        {
            InitializeComponent();
            if (this.DataContext is ViewModels.AddEditUserViewModel vm)
            {
                vm.Close += () => this.Close();
                vm.Users = items;
                vm.tag = tag;
                vm.SelectedItem= SelectedCollection;
            }
        }
    }
    }
