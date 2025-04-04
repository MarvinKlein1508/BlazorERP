﻿using Dapper;
using Npgsql;
using System.Data;

namespace BlazorERP.Core.Modules.CoreData;
public class UserService
{
    public Task<User?> GetAsync(string username, AccountType accountType, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    { 
        CommandDefinition command = new CommandDefinition(
            commandText: "SELECT * FROM users WHERE username = @Username AND account_type = @AccountType",
            parameters: new { Username = username, AccountType = accountType },
            transaction: transaction,
            commandType: CommandType.Text,
            cancellationToken: cancellationToken
        );

        return connection.QueryFirstOrDefaultAsync<User>(command);
    }
}
