using BlazorERP.Core.Enums;
using BlazorERP.Core.Interfaces;
namespace BlazorERP.Core.Models;
public class User : IDbModelWithName<int?>
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername => Username.ToUpper();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public Guid? ActiveDirectoryGuid { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public AccountType AccountType { get; set; } = AccountType.LocalAccount;
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }

    public int? GetIdentifier() => UserId;
    public string GetName() => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// This property is only to compare passwords during administration processes.
    /// </summary>
    public string PasswordConfirm { get; set; } = string.Empty;
}
