using Microsoft.AspNetCore.Identity;

namespace BlazorERP.Core.Models;
public class ApplicationUser : IdentityUser<int>
{
    public Guid? ActiveDirectoryGuid { get; set; }

    public bool IsActive { get; set; }
    public bool IsActiveDirectoryUser => ActiveDirectoryGuid is not null;
}
