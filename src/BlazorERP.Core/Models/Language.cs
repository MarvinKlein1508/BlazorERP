using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Language : IDbModel<int?>
{
    public int LanguageId { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }

    public int? GetIdentifier() => LanguageId > 0 ? LanguageId : null;

    public List<Translation> Translations { get; set; } = [];
    public string GetName(int languageId)
    {
        var translation = Translations.First(x => x.LanguageId == languageId);
        return translation.ValueText;
    }

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "LANGUAGE_ID", LanguageId },
            { "CODE", Code },
            { "IS_ACTIVE", IsActive },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }
}
