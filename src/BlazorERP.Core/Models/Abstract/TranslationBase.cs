using BlazorERP.Core.Utilities;

namespace BlazorERP.Core.Models.Abstract;

public abstract class TranslationBase
{
    public List<Translation> Translations { get; set; } = [];
    public string GetName(int languageId)
    {
        var translation = Translations.FirstOrDefault(x => x.LanguageId == languageId);

        if (translation is null)
        {
            translation = Translations.FirstOrDefault(x => x.LanguageId == Storage.DEFAULT_LANGUAGE);
        }

        return translation?.ValueText ?? string.Empty;
    }
}
