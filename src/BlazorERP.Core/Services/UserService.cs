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
                DISPLAY_NAME,
                ACTIVE_DIRECTORY_GUID,
                EMAIL,
                PASSWORD,
                SALT,
                ACCOUNT_TYPE,
                IS_ACTIVE,
                IS_ADMIN,
                CREATED_AT,
                CREATED_BY,
                LAST_MODIFIED_BY,
                LAST_MODIFIED
            )
            VALUES
            (
                @USERNAME,
                @NORMALIZED_USERNAME,
                @FIRSTNAME,
                @LASTNAME,
                @DISPLAY_NAME,
                @ACTIVE_DIRECTORY_GUID,
                @EMAIL,
                @PASSWORD,
                @SALT,
                @ACCOUNT_TYPE,
                @IS_ACTIVE,
                @IS_ADMIN,
                @CREATED_AT,
                @CREATED_BY,
                @LAST_MODIFIED_BY,
                @LAST_MODIFIED
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

        string sql =
            """
            SELECT 
                U.*,
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName
            FROM USERS U
            LEFT JOIN USERS UC ON (UC.USER_ID = U.CREATED_BY)
            LEFT JOIN USERS UL ON (UL.USER_ID = U.LAST_MODIFIED_BY)
            WHERE 
                USER_ID = @USER_ID
            """;

        return dbController.GetFirstAsync<User>(sql,
        new
        {
            USER_ID = userId
        }, cancellationToken);
    }
    public Task<User?> GetAsync(Guid guid, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return dbController.GetFirstAsync<User>("SELECT * FROM USERS WHERE ACTIVE_DIRECTORY_GUID = @ACTIVE_DIRECTORY_GUID", new
        {
            ACTIVE_DIRECTORY_GUID = guid.ToString()
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
                DISPLAY_NAME = @DISPLAY_NAME,
                EMAIL = @EMAIL,
                IS_ACTIVE = @IS_ACTIVE,
                IS_ADMIN = @IS_ADMIN,
                CREATED_AT = @CREATED_AT,
                CREATED_BY = @CREATED_BY,
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY,
                LAST_MODIFIED = @LAST_MODIFIED
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
                U.*,
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName 
            FROM USERS U
            LEFT JOIN USERS UC ON (UC.USER_ID = U.CREATED_BY)
            LEFT JOIN USERS UL ON (UL.USER_ID = U.LAST_MODIFIED_BY)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY USER_ID DESC
        """;



        List<User> list = await dbController.SelectDataAsync<User>(sql, filter.GetParameters(), cancellationToken);

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


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public string GetFilterWhere(UserFilter filter)
    {
        StringBuilder sb = new();

        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@" AND 
(
        UPPER(FIRSTNAME) LIKE @SEARCH_PHRASE
    OR  UPPER(LASTNAME) LIKE @SEARCH_PHRASE
    OR  NORMALIZED_USERNAME LIKE @SEARCH_PHRASE
)");
        }



        string sql = sb.ToString();
        return sql;
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

    public Task<List<User>> GetAsync(int?[] identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
