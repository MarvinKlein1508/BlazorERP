using Dapper;
using System.Data;

namespace BlazorERP.Core.Interfaces;

/// <summary>
/// Generalization of a database interface
/// </summary>
public interface IDbController : IDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction? Transaction { get; }
    /// <summary>
    /// Executes SQL and returns the first specified object found.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>When no object is found, this method will return null.</returns>
    Task<T?> GetFirstAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default);
    /// <summary>
    /// Executes SQL and returns all found objects within a list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>When no objects are found, an empty list will be returned.</returns>
    Task<List<T>> SelectDataAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default);
    /// <summary>
    /// Executes SQL and does not return anything.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task QueryAsync(string sql, object? param = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the provided procedure.
    /// </summary>
    /// <param name="procedureName"></param>
    /// <param name="param"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DynamicParameters?> ExecuteProcedureAsync(string procedureName, DynamicParameters? param = null, CancellationToken cancellationToken = default);
}

