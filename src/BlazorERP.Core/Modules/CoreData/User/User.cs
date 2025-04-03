namespace BlazorERP.Core.Modules.CoreData;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid? ActiveDirectoryGuid { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Salt { get; set; }
    public bool IsActive { get; set; }
    public AccountType AccountType { get; set; } = AccountType.LocalAccount;
}
