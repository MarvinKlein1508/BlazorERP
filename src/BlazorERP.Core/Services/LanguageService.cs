using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;

namespace BlazorERP.Core.Services;

public class LanguageService : ITranslationCode
{
    private readonly TranslationService _translationService;

    public LanguageService(TranslationService translationService)
    {
        _translationService = translationService;
    }


    public static async Task<List<Language>> GetAsync(IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string sql = "SELECT * FROM LANGUAGES";


        var results = await dbController.SelectDataAsync<Language>(sql, null, cancellationToken);

        var translations = await TranslationService.GetAsync(GetTranslationCode(), dbController);

        foreach (var item in results)
        {
            item.Translations = translations.Where(x => x.ParentId == item.LanguageId).ToList();
        }

        return results;
    }

    public static string GetTranslationCode() => "LANGUAGE";
}
