using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class UserService : IModelService<User, int?, UserFilter>
{
    public Task CreateAsync(User input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(User input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetAsync(int? userId, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (userId is null)
        {
            return Task.FromResult<User?>(null);
        }

        string sql = "SELECT * FROM USERS WHERE USER_ID = @USER_ID";

        return dbController.GetFirstAsync<User>(sql,
        new
        {
            USER_ID = userId
        }, cancellationToken);
    }
    public Task UpdateAsync(User input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }


    public async Task<User?> GetByUsernameAsync(string username, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "SELECT * FROM USERS WHERE NORMALIZED_USERNAME = @USERNAME";

        var result = await dbController.GetFirstAsync<User>(sql,
        new
        {
            USERNAME = username.ToUpper()
        }, cancellationToken);


        return result;
    }

    public async Task<List<User>> GetAsync(UserFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                * 
            FROM USERS 
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY USER_ID DESC
        """;



        List<User> list = await dbController.SelectDataAsync<User>(sql, GetFilterParameter(filter), cancellationToken);

        // TODO: Load permissions

        return list;
    }

    public Task<int> GetTotalAsync(UserFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM USERS
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, GetFilterParameter(filter), cancellationToken);
    }

    public string GetFilterWhere(UserFilter filter)
    {
        StringBuilder sb = new();

        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@" AND 
(
        UPPER(FIRSTNAME) LIKE @SEARCHPHRASE
    OR  UPPER(LASTNAME) LIKE @SEARCHPHRASE
    OR  NORMALIZED_USERNAME LIKE @SEARCHPHRASE
)");
        }



        string sql = sb.ToString();
        return sql;
    }

    public Dictionary<string, object?> GetFilterParameter(UserFilter filter)
    {
        return new Dictionary<string, object?>
        {
            { "SEARCHPHRASE", $"%{filter.SearchPhrase}%" }
        };
    }
}
