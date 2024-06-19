using BlazorERP.Application.Models;

namespace BlazorERP.Application.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> CreateAsync(User user, CancellationToken token = default);
    Task<bool> UpdateAsync(User user, CancellationToken token = default);
    Task<User?> GetAsync(int userId, CancellationToken token = default);
}
