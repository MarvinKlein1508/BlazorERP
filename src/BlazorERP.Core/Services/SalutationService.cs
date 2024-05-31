using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;
using System.Threading;

namespace BlazorERP.Core.Services;

public class SalutationService : IModelService<Salutation, int?, SalutationFilter>, ITranslationCode
{
    private readonly TranslationService _translationService;

    public SalutationService(TranslationService translationService)
    {
        _translationService = translationService;
    }

    public async Task CreateAsync(Salutation input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO SALUTATIONS
            (
                LAST_MODIFIED_BY,
                LAST_MODIFIED
            )
            VALUES
            (
                @LAST_MODIFIED_BY,
                @LAST_MODIFIED
            ) RETURNING SALUTATION_ID;
            """;



        input.SalutationId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

        foreach (var item in input.Translations)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.SalutationId;

            await _translationService.CreateAsync(item, dbController, cancellationToken);
        }

    }

    public Task DeleteAsync(Salutation input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM SALUTATIONS WHERE SALUTATION_ID = @SALUTATION_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public static async Task<List<Salutation>> GetAsync(IDbController dbController)
    {
        string sql = "SELECT * FROM SALUTATIONS";

        var results = await dbController.SelectDataAsync<Salutation>(sql);


        var translations = await TranslationService.GetAsync(GetTranslationCode(), dbController);

        foreach (var item in results)
        {
            item.Translations = translations.Where(x => x.ParentId == item.SalutationId).ToList();
        }

        return results;
    }
    public async Task<Salutation?> GetAsync(int? salutationId, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (salutationId is null)
        {
            return null;
        }

        string sql =
            """
            SELECT 
                S.*,
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName
            FROM SALUTATIONS S
            LEFT JOIN USERS UC ON (UC.USER_ID = S.CREATED_BY)
            LEFT JOIN USERS UL ON (UL.USER_ID = S.LAST_MODIFIED_BY)
            WHERE 
                SALUTATION_ID = @SALUTATION_ID
            """;

        var result = await dbController.GetFirstAsync<Salutation>(sql, new
        {
            SALUTATION_ID = salutationId
        }, cancellationToken);

        if (result is not null)
        {
            result.Translations = await _translationService.GetAsync(GetTranslationCode(), result.SalutationId, dbController, cancellationToken);
        }

        return result;
    }

    public async Task<List<Salutation>> GetAsync(SalutationFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                S.*,
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName
            FROM SALUTATIONS S
            LEFT JOIN USERS UC ON (UC.USER_ID = S.CREATED_BY)
            LEFT JOIN USERS UL ON (UL.USER_ID = S.LAST_MODIFIED_BY)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY SALUTATION_ID DESC
        """;

        var results = await dbController.SelectDataAsync<Salutation>(sql, filter.GetParameters(), cancellationToken);
        if (results.Count > 0)
        {
            var salutationIds = results.Select(x => x.SalutationId).ToArray();
            var translations = await _translationService.GetAsync(GetTranslationCode(), salutationIds, dbController, cancellationToken);
            foreach (var item in results)
            {
                item.Translations = translations.Where(x => x.ParentId == item.SalutationId).ToList();
            }
        }

        return results;
    }



    public string GetFilterWhere(SalutationFilter filter)
    {
        StringBuilder sb = new();

        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@" AND 
(
        UPPER(NAME) LIKE @SEARCH_PHRASE
)");
        }



        string sql = sb.ToString();
        return sql;
    }

    public Task<int> GetTotalAsync(SalutationFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM SALUTATIONS
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    

    public async Task UpdateAsync(Salutation input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE SALUTATIONS SET 
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY,
                LAST_MODIFIED = @LAST_MODIFIED
            WHERE
                SALUTATION_ID = @SALUTATION_ID
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);


        await _translationService.ClearAsync(GetTranslationCode(), input.SalutationId, dbController, cancellationToken);
        foreach (var item in input.Translations)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.SalutationId;

            await _translationService.CreateAsync(item, dbController, cancellationToken);
        }
    }

    public static string GetTranslationCode() => "SALUTATION";

    public Task<List<Salutation>> GetAsync(int?[] identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
