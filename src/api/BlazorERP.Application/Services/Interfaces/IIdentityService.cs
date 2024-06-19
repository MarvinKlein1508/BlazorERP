using BlazorERP.Application.Models;

namespace BlazorERP.Application.Services.Interfaces;

public interface IIdentityService
{
    Task<bool> LoginAsync(User user, CancellationToken cancellationToken = default);
}
