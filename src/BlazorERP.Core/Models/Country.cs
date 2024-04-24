using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models.Abstract;

namespace BlazorERP.Core.Models;

public class Country : TranslationBase, IDbModel<int?>
{
    public int CountryId { get; set; }
    public string ISO2 { get; set; } = string.Empty;
    public string ISO3 { get; set; } = string.Empty;
    public string DialingCode { get; set; } = string.Empty;

    public bool IsEU { get; set; }
    public bool IsActive { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }

    public int? GetIdentifier() => CountryId > 0 ? CountryId : null;



    public string BearbeiterName { get; set; } = string.Empty;
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "COUNTRY_ID", CountryId },
            { "ISO2", ISO2 },
            { "ISO3", ISO3 },
            { "DIALING_CODE", DialingCode },
            { "IS_EU", IsEU },
            { "IS_ACTIVE", IsActive },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }
}
