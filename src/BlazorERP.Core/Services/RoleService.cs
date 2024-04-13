using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;

namespace BlazorERP.Core.Services;

public class RoleService : IModelService<Role, int, RoleFilter>
{
    public Task CreateAsync(Role input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Role input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Role?> GetAsync(int identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Role>> GetAsync(RoleFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }


    public string GetFilterWhere(RoleFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTotalAsync(RoleFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Role input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
