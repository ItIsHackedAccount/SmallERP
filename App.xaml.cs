using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Windows;
using ERP.Data;

namespace ERP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 注册数据库提供程序
            //DatabaseClient.RegisterProviderIfAvailable("Microsoft.Data.SqlClient", Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            //DatabaseClient.RegisterProviderIfAvailable("Microsoft.Data.Sqlite", Microsoft.Data.Sqlite.SqliteFactory.Instance);
        //    DatabaseClient.RegisterProviderIfAvailable("Oracle.ManagedDataAccess.Client", "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess.Core");

        }
        static App()
        {
            // 在类加载时执行一次，保证设计器和运行时都能识别
            DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", Microsoft.Data.Sqlite.SqliteFactory.Instance);
            DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", Microsoft.Data.SqlClient.SqlClientFactory.Instance);
        }



    }
}
