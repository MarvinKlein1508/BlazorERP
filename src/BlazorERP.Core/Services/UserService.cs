using BlazorERP.Core.Extensions;
using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;
using Microsoft.AspNetCore.Identity;
namespace BlazorERP.Core.Services;

public class UserService : IModelService<User, int?, UserFilter>
{
    public async Task CreateAsync(User input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO USERS
            (
                USERNAME,
                NORMALIZED_USERNAME,
                FIRSTNAME,
                LASTNAME,
                ACTIVE_DIRECTORY_GUID,
                EMAIL,
                PASSWORD,
                SALT,
                ACCOUNT_TYPE,
                IS_ACTIVE,
                IS_ADMIN
            )
            VALUES
            (
                @USERNAME,
                @NORMALIZED_USERNAME,
                @FIRSTNAME,
                @LASTNAME,
                @ACTIVE_DIRECTORY_GUID,
                @EMAIL,
                @PASSWORD,
                @SALT,
                @ACCOUNT_TYPE,
                @IS_ACTIVE,
                @IS_ADMIN
            ) RETURNING USER_ID;
            """;



        input.UserId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
    }

    public Task DeleteAsync(User input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql = "DELETE FROM USERS WHERE USER_ID = @USER_ID";
        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
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
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            UPDATE USERS SET
                USERNAME = @USERNAME,
                NORMALIZED_USERNAME = @NORMALIZED_USERNAME,
                FIRSTNAME = @FIRSTNAME,
                LASTNAME = @LASTNAME,
                EMAIL = @EMAIL,
                IS_ACTIVE = @IS_ACTIVE,
                IS_ADMIN = @IS_ADMIN
            WHERE
                USER_ID = @USER_ID
            """;

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
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

    public async Task<bool> ChangePasswordAsync(ChangePasswordModel input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await GetAsync(input.UserId, dbController, cancellationToken);
        if (user is null)
        {
            throw new NullReferenceException(nameof(user));
        }

        PasswordHasher<User> hasher = new();
        // Check old password
        if (input.RequireOldPassword)
        {
            PasswordVerificationResult result = hasher.VerifyHashedPassword(user, user.Password, input.PasswordOld + user.Salt);
            if (result is PasswordVerificationResult.Failed)
            {
                return false;
            }
        }

        user.Salt = StringExtensions.RandomString(20);

        string passwordHashed = hasher.HashPassword(user, input.PasswordNew + user.Salt);

        user.Password = passwordHashed;

        string sql =
            """
            UPDATE USERS SET
               PASSWORD = @PASSWORD,
               SALT = @SALT
            WHERE 
                USER_ID = @USER_ID
            """;

        await dbController.QueryAsync(sql, user.GetParameters(), cancellationToken);

        return true;
    }
}
