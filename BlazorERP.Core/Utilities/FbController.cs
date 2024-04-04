using BlazorERP.Core.Interfaces;
using Dapper;
using FirebirdSql.Data.FirebirdClient;

namespace BlazorERP.Core.Utilities;

public sealed class FbController : IDbController<FbConnection, FbTransaction>
{
    public FbConnection Connection { get; init; }
    public FbTransaction? Transaction { get; }

    public FbController(string connectionString)
    {
        Connection = new FbConnection(connectionString);
        Connection.Open();
    }
    public Task CommitAsync()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task<DynamicParameters?> ExecuteProcedureAsync(string procedureName, DynamicParameters? param = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetFirstAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task QueryAsync(string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RollbackAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<T>> SelectDataAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task StartTransactionAsync()
    {
        throw new NotImplementedException();
    }
}
