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
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }

    public int? GetIdentifier() => ContactPersonId > 0 ? ContactPersonId : null;
    public string GetName()
    {
        return $"{FirstName} {LastName}".Trim();
    }
    public string CreatedByName { get; set; } = string.Empty;
    public string LastModifiedName { get; set; } = string.Empty;
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "CONTACT_PERSON_ID", ContactPersonId },
            { "COMPANY", Company },
            { "SALUTATION_ID", SalutationId },
            { "FIRSTNAME", FirstName },
            { "LASTNAME", LastName },
            { "DEPARTMENT", Department },
            { "LANGUAGE_ID", LanguageId },
            { "PHONE_NUMBER", PhoneNumber },
            { "MOBILE_NUMBER", MobileNumber },
            { "MOBILE_NUMBER", MobileNumber },
            { "FAX_NUMBER", FaxNumber },
            { "EMAIL", Email },
            { "NOTE", Note },
            { "CREATED_AT", CreatedAt },
            { "CREATED_BY", CreatedBy },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }
}
