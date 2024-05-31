using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models.Abstract;

namespace BlazorERP.Core.Models;

public class Language : TranslationBase, IDbModel<int?>
{
    public int LanguageId { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }

    public string CreatedByName { get; set; } = string.Empty;
    public string LastModifiedName { get; set; } = string.Empty;

    public int? GetIdentifier() => LanguageId > 0 ? LanguageId : null;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "LANGUAGE_ID", LanguageId },
            { "CODE", Code },
            { "IS_ACTIVE", IsActive },
            { "CREATED_AT", CreatedAt },
            { "CREATED_BY", CreatedBy },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }
}
