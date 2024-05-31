using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models.Abstract;

namespace BlazorERP.Core.Models;

public class DeliveryCondition : TranslationBase, IDbModel<int?>
{   
    public int DeliveryConditionId { get; set; }
    public bool ShippingByCarrier { get; set; }
    public bool IsPickup { get; set; }
    public bool AvailableForCustomer { get; set; }
    public bool AvailableForSupplier { get; set; }
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }


    public int? GetIdentifier() => DeliveryConditionId > 0 ? DeliveryConditionId : null;

   
    public string CreatedByName { get; set; } = string.Empty;
    public string LastModifiedName { get; set; } = string.Empty;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "DELIVERY_CONDITION_ID", DeliveryConditionId },
            { "SHIPPING_BY_CARRIER", ShippingByCarrier },
            { "IS_PICKUP", IsPickup },
            { "AVAILABLE_FOR_CUSTOMER", AvailableForCustomer },
            { "AVAILABLE_FOR_SUPPLIER", AvailableForSupplier },
            { "IS_ACTIVE", IsActive },
            { "CREATED_AT", CreatedAt },
            { "CREATED_BY", CreatedBy },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }
}
