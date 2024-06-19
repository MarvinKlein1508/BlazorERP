using BlazorERP.Application.Database;
using BlazorERP.Application.Models;
using BlazorERP.Application.Repositories.Interfaces;
using Dapper;

namespace BlazorERP.Application.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public UserRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    public Task<bool> CreateAsync(User user, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetAsync(int userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        string sql =
            """
            SELECT 
                U.*
            FROM USERS U
            WHERE 
                U.USER_ID = @userId
            """;

        var command = new CommandDefinition(sql, new { userId }, cancellationToken: token);

        var user = await connection.QuerySingleOrDefaultAsync<User>(command);

        if (user is null)
        {
            return null;
        }

        return user;
    }

    public Task<bool> UpdateAsync(User user, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
