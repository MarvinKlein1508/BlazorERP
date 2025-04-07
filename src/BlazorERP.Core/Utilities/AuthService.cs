using BlazorERP.Core.Modules.CoreData;
using Microsoft.AspNetCore.Components.Authorization;
using System.Data;
using System.Security.Claims;

namespace BlazorERP.Core.Utilities;

public class AuthService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly UserService _userService;
    private readonly IDbConnectionFactory _dbFactory;

    public AuthService(AuthenticationStateProvider authenticationStateProvider, UserService userService, IDbConnectionFactory dbFactory)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _userService = userService;
        _dbFactory = dbFactory;
    }
    /// <summary>
    /// Converts the active claims into a <see cref="User"/> object
    /// </summary>
    /// <returns></returns>
    public async Task<User?> GetUserAsync(IDbConnection? connection = null)
    {

        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity is not null && user.Identity.IsAuthenticated)
        {
            Claim? claim = user.FindFirst("userId");
            if (claim is null)
            {
                return null;
            }

            var userId = Convert.ToInt32(claim.Value);

            bool shouldDispose = connection is null;


            connection ??= await _dbFactory.CreateConnectionAsync();

            var result = await _userService.GetAsync(userId, connection);

            if (shouldDispose)
            {
                connection.Dispose();
            }

            return result;
        }

        return null;
    }

    /// <summary>
    /// Checks if the currently logged in user as a specific role within it's claims.
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<bool> HasRole(string roleName)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        return user.IsInRole(roleName);
    }

    public async Task<Dictionary<string, bool>> CheckRoles(params string[] roleNames)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        Dictionary<string, bool> results = [];

        foreach (var roleName in roleNames)
        {
            results.Add(roleName, user.IsInRole(roleName));
        }

        return results;

    }
}
