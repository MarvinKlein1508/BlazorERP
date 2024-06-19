using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorERP.Core.Services;

public class LanguageService : ITranslationCode, IGetOperation<Language, int?>
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

    public async Task<Language?> GetAsync(int? languageId, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string sql =
            """
            SELECT 
                L.*,
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName
            FROM LANGUAGES L 
            LEFT JOIN USERS UC ON (UC.USER_ID = L.CREATED_BY)
            LEFT JOIN USERS UL ON (UL.USER_ID = L.LAST_MODIFIED_BY)
            WHERE LANGUAGE_ID = @LANGUAGE_ID
            """;

        var result = await dbController.GetFirstAsync<Language>(sql, new
        {
            LANGUAGE_ID = languageId,
        }, cancellationToken);

        if (result is not null)
        {
            result.Translations = await _translationService.GetAsync(GetTranslationCode(), result.LanguageId, dbController, cancellationToken);
        }

        return result;
    }

    public async Task<List<Language>> GetAsync(int?[] identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (identifiers.Length is 0)
        {
            return [];
        }

        var parameters = new Dictionary<string, object?>();
        List<string> parameterNames = [];

        for (int i = 0; i < identifiers.Length; i++)
        {
            string parameterName = $"@LANGUAGE_ID{i}";
            parameterNames.Add(parameterName);
            parameters.Add(parameterName, identifiers[i]);
        }

        string parameterQuery = string.Join(", ", parameterNames);

        string sql =
        $"""
        SELECT 
            L.*,
            UC.DISPLAY_NAME AS CreatedByName,
            UL.DISPLAY_NAME AS LastModifiedName
        FROM LANGUAGES L 
        LEFT JOIN USERS UC ON (UC.USER_ID = L.CREATED_BY)
        LEFT JOIN USERS UL ON (UL.USER_ID = L.LAST_MODIFIED_BY)
        WHERE 1 = 1 AND LANGUAGE_ID IN 
        (
            {parameterQuery}
        )
        ORDER BY LANGUAGE_ID
        """;

        var results = await dbController.SelectDataAsync<Language>(sql, parameters, cancellationToken);
        if (results.Count > 0)
        {
            var languageIds = results.Select(x => x.LanguageId).Distinct().ToArray();
            var translations = await _translationService.GetAsync(GetTranslationCode(), languageIds, dbController, cancellationToken);

            foreach (var item in results)
            {
                item.Translations = translations.Where(x => x.ParentId == item.LanguageId).ToList();
            }
        }

        return results;
    }
}
