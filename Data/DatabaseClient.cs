using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ERP.Data
{
    /// <summary>
    /// 通用数据库客户端，基于 DbProviderFactory，支持 SqlServer/SQLite/Oracle 等通过 provider 注册的 ADO.NET 提供程序。
    /// 需要在程序启动时确保对应提供程序已通过 NuGet 安装并注册到 DbProviderFactories（示例见 RegisterProviderIfAvailable）。
    /// </summary>
    public sealed class DatabaseClient : IDisposable
    {
        private readonly DbProviderFactory _factory;
        private readonly string _connectionString;

        public DatabaseClient(string providerInvariantName, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(providerInvariantName)) throw new ArgumentNullException(nameof(providerInvariantName));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _factory = DbProviderFactories.GetFactory(providerInvariantName) ?? throw new InvalidOperationException($"Provider not found: {providerInvariantName}");
        }

        /// <summary>
        /// 尝试根据运行时类型自动注册常见提供程序的 factory（如果所需包已引用）。
        /// 示例调用：
        /// RegisterProviderIfAvailable("Microsoft.Data.SqlClient", "Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient");
        /// RegisterProviderIfAvailable("Microsoft.Data.Sqlite", "Microsoft.Data.Sqlite.SqliteFactory, Microsoft.Data.Sqlite");
        /// RegisterProviderIfAvailable("Oracle.ManagedDataAccess.Client", "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess.Core");
        /// </summary>
        public static void RegisterProviderIfAvailable(string invariantName, DbProviderFactory factory)
        {
            if (factory != null)
            {
                DbProviderFactories.RegisterFactory(invariantName, factory);
            }
        }
        private DbConnection CreateConnection()
        {
            var conn = _factory.CreateConnection() ?? throw new InvalidOperationException("Factory returned null connection");
            conn.ConnectionString = _connectionString;
            return conn;
        }

        public DbParameter CreateParameter(string name, object? value)
        {
            var p = _factory.CreateParameter() ?? throw new InvalidOperationException("Factory returned null parameter");
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            return p;
        }

        public async Task<int> ExecuteNonQueryAsync( string sql,IEnumerable<DbParameter>? parameters = null,CancellationToken cancellationToken = default)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync(cancellationToken);

            await using var transaction = await conn.BeginTransactionAsync(cancellationToken);

            try
            {
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.Transaction = transaction;   // 关键：绑定事务

                if (parameters != null)
                {
                    foreach (var p in parameters)
                        cmd.Parameters.Add(p);
                }

                var affected = await cmd.ExecuteNonQueryAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken); // 提交事务
                return affected;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken); // 出错回滚
                throw;
            }
        }

        public async Task<object?> ExecuteScalarAsync(string sql, IEnumerable<DbParameter>? parameters = null, CancellationToken cancellationToken = default)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync(cancellationToken);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var p in parameters) cmd.Parameters.Add(p);
            }
            return await cmd.ExecuteScalarAsync(cancellationToken);
        }

        public async Task<List<Dictionary<string, object?>>> ExecuteQueryAsync(string sql, IEnumerable<DbParameter>? parameters = null, CancellationToken cancellationToken = default)
        {
            var result = new List<Dictionary<string, object?>>();
            await using var conn = CreateConnection();
            await conn.OpenAsync(cancellationToken);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var p in parameters) cmd.Parameters.Add(p);
            }

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            var fieldCount = reader.FieldCount;
            while (await reader.ReadAsync(cancellationToken))
            {
                var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < fieldCount; i++)
                {
                    row[reader.GetName(i)] = await reader.IsDBNullAsync(i, cancellationToken) ? null : reader.GetValue(i);
                }
                result.Add(row);
            }

            return result;
        }

        public void Dispose()
        {
            // 无需释放 factory；此类不持有持续连接
            GC.SuppressFinalize(this);
        }
    }
}