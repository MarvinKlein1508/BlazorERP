using BlazorERP.Application.Models;
using BlazorERP.Application.Repositories.Interfaces;
using BlazorERP.Application.Services.Interfaces;

namespace BlazorERP.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public Task<User?> GetAsync(int userId, CancellationToken token = default)
    {
        return _userRepository.GetAsync(userId, token);
    }
}
