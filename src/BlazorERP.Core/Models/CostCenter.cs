using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class CostCenter : IDbModelWithName<int?>
{
    public int CostCenterId { get; set; }
    public string CostCenterNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }

    public int? GetIdentifier() => CostCenterId > 0 ? CostCenterId : null;
    public string GetName() => Name;
    public string CreatedByName { get; set; } = string.Empty;
    public string LastModifiedName { get; set; } = string.Empty;
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "COST_CENTER_ID", CostCenterId },
            { "COST_CENTER_NUMBER", CostCenterNumber },
            { "NAME", Name },
            { "CREATION_DATE", CreationDate },
            { "CREATED_AT", CreatedAt },
            { "CREATED_BY", CreatedBy },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }
}
