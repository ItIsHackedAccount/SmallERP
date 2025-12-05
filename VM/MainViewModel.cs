
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ERP.Helpers;
using ERP.Views;

namespace ERP.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isMenuExpanded = true;
        public bool IsMenuExpanded
        {
            get => _isMenuExpanded;
            set { _isMenuExpanded = value; OnPropertyChanged(); }
        }

        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        private object? _userName;
        public object? UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        public ICommand ToggleMenuCommand { get; }
        public ICommand ShowInventoryCommand { get; }
        public ICommand ShowSalesCommand { get; }
        public ICommand ShowPurchaseCommand { get; }
        public ICommand ShowFinanceCommand { get; }
        public ICommand ShowUserManagementCommand { get; }

        public ICommand LogOutCommand { get; }

        public ICommand MaximizeWindowCommand { get; }

        public ICommand MinimizeWindowCommand { get; }

        public ICommand CloseWindowCommand { get; }

        public ICommand ShowWorkOrderCommand { get; }

        public ICommand ShowDispatchCommand { get; }

        public event Action RequestClose;

    
        public event Action RequestMaximize;

        public event Action RequestMinimize;

        public MainViewModel()
        {
            ToggleMenuCommand = new RelayCommand(_ => IsMenuExpanded = !IsMenuExpanded);
            ShowInventoryCommand = new RelayCommand(_ => CurrentView = new InventoryViewModel());
            ShowSalesCommand = new RelayCommand(_ => CurrentView = new SalesViewModel());
            ShowPurchaseCommand = new RelayCommand(_ => CurrentView = new PurchaseViewModel());
            ShowFinanceCommand = new RelayCommand(_ => CurrentView = new FinanceViewModel());
            ShowUserManagementCommand = new RelayCommand(_ => CurrentView = new UserManagementViewModel());
          //  ShowWorkOrderCommand = new RelayCommand(_ => CurrentView = new WorkOrderViewModel());
          ShowDispatchCommand= new RelayCommand(_ => CurrentView = new DispatchReportViewModel());

            // 默认页面
            CurrentView = new InventoryViewModel();

            LogOutCommand = new RelayCommand(_=>LogOut());

            MaximizeWindowCommand = new RelayCommand(_ => RequestMaximize?.Invoke());

            MinimizeWindowCommand = new RelayCommand(_ => RequestMinimize?.Invoke());

            CloseWindowCommand = new RelayCommand(_ => RequestClose?.Invoke());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        public void LogOut()
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.RequestClose?.Invoke();
       
        }
    }

}