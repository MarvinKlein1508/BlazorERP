using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;

namespace BlazorERP.Core.Services;

public class UserService : IModelService<User, int>
{
    public Task CreateAsync(User input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(User input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetAsync(int userId, IDbController dbController, CancellationToken cancellationToken = default)
    {
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
}
