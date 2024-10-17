using BlazorERP.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorERP.Core.Extensions;
public static class UserManagerExtensions
{
    public static async Task<ApplicationUser?> GetByActiveDirectoryGuidAsync(this UserManager<ApplicationUser> userManager, Guid guid)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.ActiveDirectoryGuid == guid);

        return user;
    }
}
