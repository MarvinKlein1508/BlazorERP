namespace BlazorERP.Application.Models;

public class User
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public string NormalizedUsername => Username.ToUpper();
    public required string Firstname { get; set; } 
    public required string Lastname { get; set; } 
    public required Guid? ActiveDirectoryGuid { get; set; }
    public required string Email { get; set; } 
    public required string Password { get; set; } 
    public required string Salt { get; set; } 
    //public AccountType AccountType { get; set; } = AccountType.LocalAccount;
    public required bool IsAdmin { get; set; }
    public required bool IsActive { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required int? CreatedBy { get; set; }
    public required int? LastModifiedBy { get; set; }
    public required DateTime? LastModified { get; set; }
}
