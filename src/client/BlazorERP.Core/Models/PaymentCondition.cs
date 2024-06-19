using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models.Abstract;

namespace BlazorERP.Core.Models;

public class PaymentCondition : TranslationBase, IDbModel<int?>
{   
    public int PaymentConditionId { get; set; }
    public int NetDays { get; set; }
    public int Discount1Days { get; set; }
    public decimal Discount1Percent { get; set; }
    public int Discount2Days { get; set; }
    public decimal Discount2Percent { get; set; }
    public bool IsPrepayment { get; set; }
    public bool IsCashPayment { get; set; }
    public bool IsDirectDebit { get; set; }
    public bool IsInvoice { get; set; }
    public bool IsActive { get; set; }

    public bool AvailableForCustomer { get; set; }
    public bool AvailableForSupplier { get; set; }

    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }

    public int? GetIdentifier() => PaymentConditionId > 0 ? PaymentConditionId : null;

    public string CreatedByName { get; set; } = string.Empty;
    public string LastModifiedName { get; set; } = string.Empty;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "PAYMENT_CONDITION_ID", PaymentConditionId },
            { "NET_DAYS", NetDays },
            { "DISCOUNT1_DAYS", Discount1Days },
            { "DISCOUNT1_PERCENT", Discount1Percent },
            { "DISCOUNT2_DAYS", Discount2Days },
            { "DISCOUNT2_PERCENT", Discount2Percent },
            { "IS_PREPAYMENT", IsPrepayment },
            { "IS_CASH_PAYMENT", IsCashPayment },
            { "IS_DIRECT_DEBIT", IsDirectDebit },
            { "IS_INVOICE", IsInvoice },
            { "IS_ACTIVE", IsActive },
            { "AVAILABLE_FOR_CUSTOMER", AvailableForCustomer },
            { "AVAILABLE_FOR_SUPPLIER", AvailableForSupplier },
            { "CREATED_AT", CreatedAt },
            { "CREATED_BY", CreatedBy },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }
}
