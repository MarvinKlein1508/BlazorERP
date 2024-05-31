using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class NumberRange : IDbModelWithName<int?>
{
    public int NumberRangeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CustomerFrom { get; set; }
    public int CustomerTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public int? GetIdentifier() => NumberRangeId > 0 ? NumberRangeId : null;

    public string GetName() => Name;

    public string CreatedByName { get; set; } = string.Empty;
    public string LastModifiedName { get; set; } = string.Empty;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "NUMBER_RANGE_ID", NumberRangeId },
            { "NAME", Name },
            { "CUSTOMER_FROM", CustomerFrom },
            { "CUSTOMER_TO", CustomerTo },
            { "CREATED_AT", CreatedAt },
            { "CREATED_BY", CreatedBy },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }
}
