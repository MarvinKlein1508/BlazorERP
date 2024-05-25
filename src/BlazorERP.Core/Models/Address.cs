using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Address : IDbModel<int?>
{
    public int AddressId { get; set; }
    public string? CustomerNumber { get; set; }
    public int? SupplierNumber { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Name1 { get; set; } = string.Empty;
    public string Name2 { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int? CountryId { get; set; }
    public int? LanguageId { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string FaxNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? ContactPersonId { get; set; }
    public string VatIdentificationNumber { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }

    public int? GetIdentifier() => AddressId > 0 ? AddressId : null;

    public string GetName()
    {
        if (!string.IsNullOrWhiteSpace(Company))
        {
            return Company;
        }

        if (!string.IsNullOrWhiteSpace(Name1))
        {
            return Name1;
        }

        return string.Empty;
    }
    public string BearbeiterName { get; set; } = string.Empty;
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "ADDRESS_ID", AddressId },
            { "CUSTOMER_NUMBER", CustomerNumber },
            { "SUPPLIER_NUMBER", SupplierNumber },
            { "COMPANY", Company },
            { "NAME1", Name1 },
            { "NAME2", Name2 },
            { "STREET", Street },
            { "COUNTRY_ID", CountryId },
            { "LANGUAGE_ID", LanguageId },
            { "POSTAL_CODE", PostalCode },
            { "CITY", City },
            { "PHONE_NUMBER", PhoneNumber },
            { "MOBILE_NUMBER", MobileNumber },
            { "FAX_NUMBER", FaxNumber },
            { "EMAIL", Email },
            { "CONTACT_PERSON_ID", ContactPersonId },
            { "VAT_IDENTIFICATION_NUMBER", VatIdentificationNumber },
            { "NOTE", Note },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified }
        };
    }
}
