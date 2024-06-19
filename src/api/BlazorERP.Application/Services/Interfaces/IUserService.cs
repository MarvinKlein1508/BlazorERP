using BlazorERP.Application.Models;

namespace BlazorERP.Application.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetAsync(int userId, CancellationToken token = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken token = default);
}
