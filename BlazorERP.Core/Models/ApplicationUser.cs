using Microsoft.AspNetCore.Identity;

namespace BlazorERP.Core.Models;
public class ApplicationUser : IdentityUser<int>
{
    public Guid? ActiveDirectoryGuid { get; set; }
}
