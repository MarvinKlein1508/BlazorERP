using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;

namespace BlazorERP.Core.Services;

public class UserService
{
    public async Task<User?> GetByUsernameAsync(string username, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "SELECT * FROM USERS WHERE NORMALIZED_USERNAME = @USERNAME";

        var result = await dbController.GetFirstAsync<User>(sql, 
        new
        {
            USERNAME = username.ToUpper()
        },cancellationToken);


        return result;
    }
}
