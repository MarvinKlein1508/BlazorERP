using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Configuration : IDbModelWithName<int?>
{
    public int ConfigurationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? DefaultLanguageId { get; set; }
    public int? CustomerDeliveryConditionId { get; set; }
    public int? CustomerPaymentConditionId { get; set; }
    public int? CustomerSalutationId { get; set; }
    public string? CustomerCurrencyCode { get; set; }
    public int? CustomerCountryId { get; set; }
    public int? CustomerLanguageId { get; set; }
    public decimal CustomerCreditLimit { get; set; }
    public bool CustomerNeutralShipping { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }

    public int? GetIdentifier() => ConfigurationId > 0 ? ConfigurationId : null;
    public string GetName() => Name;
    public string CreatedByName { get; set; } = string.Empty;
    public string LastModifiedName { get; set; } = string.Empty;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "CONFIGURATION_ID", ConfigurationId },
            { "NAME", Name },
            { "DEFAULT_LANGUAGE_ID", DefaultLanguageId },
            { "CUSTOMER_DELIVERY_CONDITION_ID", CustomerDeliveryConditionId },
            { "CUSTOMER_PAYMENT_CONDITION_ID", CustomerPaymentConditionId },
            { "CUSTOMER_SALUTATION_ID", CustomerSalutationId },
            { "CUSTOMER_CURRENCY_CODE", CustomerCurrencyCode },
            { "CUSTOMER_COUNTRY_ID", CustomerCountryId },
            { "CUSTOMER_LANGUAGE_ID", CustomerLanguageId },
            { "CUSTOMER_CREDIT_LIMIT", CustomerCreditLimit },
            { "CUSTOMER_NEUTRAL_SHIPPING", CustomerNeutralShipping },
            { "CREATED_AT", CreatedAt },
            { "CREATED_BY", CreatedBy },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }
}