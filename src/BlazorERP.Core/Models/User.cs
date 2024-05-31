using BlazorERP.Core.Enums;
using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;
public class User : IDbModelWithName<int?>
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername => Username.ToUpper();
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;

    public Guid? ActiveDirectoryGuid { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public AccountType AccountType { get; set; } = AccountType.LocalAccount;
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }

    public int? GetIdentifier() => UserId <= 0 ? null : UserId;
    public string GetName() => $"{Firstname} {Lastname}".Trim();


    public string CreatedByName { get; set; } = string.Empty;
    public string LastModifiedName { get; set; } = string.Empty;

    /// <summary>
    /// This property is only to compare passwords during administration processes.
    /// </summary>
    public string PasswordConfirm { get; set; } = string.Empty;
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "USER_ID", UserId },
            { "USERNAME", Username },
            { "NORMALIZED_USERNAME", NormalizedUsername },
            { "FIRSTNAME", Firstname },
            { "LASTNAME", Lastname },
            { "DISPLAY_NAME", GetName() },
            { "ACTIVE_DIRECTORY_GUID", ActiveDirectoryGuid?.ToString() },
            { "EMAIL", Email },
            { "PASSWORD", Password },
            { "SALT", Salt },
            { "ACCOUNT_TYPE", AccountType.ToString() },
            { "IS_ACTIVE", IsActive },
            { "IS_ADMIN", IsAdmin },
            { "CREATED_AT", CreatedAt },
            { "CREATED_BY", CreatedBy },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }

}

public class ChangePasswordModel
{
    public int UserId { get; set; }
    public string PasswordOld { get; set; } = string.Empty;
    public string PasswordNew { get; set; } = string.Empty;
    public string PasswordConfirm { get; set; } = string.Empty;

    public bool RequireOldPassword { get; set; } = true;
}
