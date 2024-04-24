using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Salutation : IDbModel<int?>
{   
    public int SalutationId { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime LastModified { get; set; }


    public int? GetIdentifier() => SalutationId > 0 ? SalutationId : null;

    public string GetName(int languageId)
    {
        var translation = Translations.First(x => x.LanguageId == languageId);
        return translation.ValueText;
    }

    public List<Translation> Translations { get; set; } = [];

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
