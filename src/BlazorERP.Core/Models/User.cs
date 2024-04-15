using BlazorERP.Core.Enums;
using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;
public class User : IDbModelWithName<int?>
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername => Username.ToUpper();
    public string Vorname { get; set; } = string.Empty;
    public string Nachname { get; set; } = string.Empty;

    public Guid? ActiveDirectoryGuid { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public AccountType AccountType { get; set; } = AccountType.LocalAccount;
    public bool IsAdmin { get; set; }
    public bool IsAktiv { get; set; }

    public int? GetIdentifier() => UserId <= 0 ? null : UserId;
    public string GetName() => $"{Vorname} {Nachname}".Trim();

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
            { "VORNAME", Vorname },
            { "NACHNAME", Nachname },
            { "ACTIVE_DIRECTORY_GUID", ActiveDirectoryGuid?.ToString() },
            { "EMAIL", Email },
            { "PASSWORD", Password },
            { "SALT", Salt },
            { "ACCOUNT_TYPE", AccountType.ToString() },
            { "IS_AKTIV", IsAktiv },
            { "IS_ADMIN", IsAdmin },
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
