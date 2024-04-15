using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;

namespace BlazorERP.Core.Services;

public class UsergruppeService : IModelService<Group, int, GroupFilter>
{
    public Task CreateAsync(Group input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Group input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Group?> GetAsync(int identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Group>> GetAsync(GroupFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public string GetFilterWhere(GroupFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTotalAsync(GroupFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Group input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
