using BlazorERP.Core.Interfaces;
using System.Net;

namespace BlazorERP.Core.Models;

public class Customer : IDbModelWithName<string?>
{
    public string CustomerNumber { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Name1 { get; set; } = string.Empty;
    public string Name2 { get; set; } = string.Empty;

    public DateTime? CreationDate { get; set; }
    public int? LanguageId { get; set; }
    public int? CountryId { get; set; }
    public Country? Country { get; set; }
    public string Street { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string FaxNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public int? SalutationId { get; set; }
    public int? PaymentConditionId { get; set; }
    public int? DeliveryConditionId { get; set; }
    public string VatIdentificationNumber { get; set; } = string.Empty;
    public decimal CreditLimit { get; set; }
    public string IBAN { get; set; } = string.Empty;
    public string BIC { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
    public bool NeutralShipping { get; set; }
    public string? CurrencyCode { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }

    public string? GetIdentifier() => string.IsNullOrWhiteSpace(CustomerNumber) ? null : CustomerNumber;

    public List<Address> Addresses { get; set; } = [];

    public string GetName()
    {
        if(!string.IsNullOrWhiteSpace(Company))
        {
            return Company;
        }

        if(!string.IsNullOrWhiteSpace(Name1))
        {
            return Name1;
        }

        return string.Empty;
    }

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "CUSTOMER_NUMBER", CustomerNumber },
            { "COMPANY", Company },
            { "NAME1", Name1 },
            { "NAME2", Name2 },
            { "CREATION_DATE", CreationDate },
            { "LANGUAGE_ID", LanguageId },
            { "COUNTRY_ID", CountryId },
            { "STREET", Street },
            { "POSTAL_CODE", PostalCode },
            { "CITY", City },
            { "PHONE_NUMBER", PhoneNumber },
            { "MOBILE_NUMBER", MobileNumber },
            { "FAX_NUMBER", FaxNumber },
            { "EMAIL", Email },
            { "WEBSITE", Website },
            { "NOTE", Note },
            { "SALUTATION_ID", SalutationId },
            { "PAYMENT_CONDITION_ID", PaymentConditionId },
            { "DELIVERY_CONDITION_ID", DeliveryConditionId },
            { "VAT_IDENTIFICATION_NUMBER", VatIdentificationNumber },
            { "CREDIT_LIMIT", CreditLimit },
            { "IBAN", IBAN },
            { "BIC", BIC },
            { "IS_BLOCKED", IsBlocked },
            { "NEUTRAL_SHIPPING", NeutralShipping },
            { "CURRENCY_CODE", CurrencyCode },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }
}
