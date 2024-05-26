using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class ContactPerson : IDbModel<int?>
{
    public int ContactPersonId { get; set; }
    public string Company { get; set; } = string.Empty;
    public int? SalutationId { get; set; }
    public Salutation? Salutation { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int? LanguageId { get; set; }
    public Language? Language { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string FaxNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }

    public int? GetIdentifier() => ContactPersonId > 0 ? ContactPersonId : null;

    public string GetName()
    {
        return $"{FirstName} {LastName}".Trim();
    }

    public Dictionary<string, object?> GetParameters()
    {
        throw new NotImplementedException();
    }
}
