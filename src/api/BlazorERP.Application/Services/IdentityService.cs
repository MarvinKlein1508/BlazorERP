using BlazorERP.Application.Models;
using BlazorERP.Application.Services.Interfaces;

namespace BlazorERP.Application.Services;

public class IdentityService : IIdentityService
{
    public Task<bool> LoginAsync(User user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
