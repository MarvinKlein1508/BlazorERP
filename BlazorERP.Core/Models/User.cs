using BlazorERP.Core.Interfaces;
namespace BlazorERP.Core.Models;
public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }

}
