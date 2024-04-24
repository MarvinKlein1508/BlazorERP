using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models.Abstract;

namespace BlazorERP.Core.Models;

public class Salutation : TranslationBase, IDbModel<int?>
{   
    public int SalutationId { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime LastModified { get; set; }

    public int? GetIdentifier() => SalutationId > 0 ? SalutationId : null;

    

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "SALUTATION_ID", SalutationId },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }
}
