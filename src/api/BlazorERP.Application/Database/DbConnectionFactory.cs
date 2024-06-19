using FirebirdSql.Data.FirebirdClient;
using System.Data;

namespace BlazorERP.Application.Database;

/// <summary>
/// Helper interface to create a new database connection.
/// </summary>
public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}

/// <summary>
/// Factory to create a new database connection to Firebird SQL.
/// </summary>
public class FirebirdConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public FirebirdConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        var connection = new FbConnection(_connectionString);
        await connection.OpenAsync(token);
        return connection;
    }
}
