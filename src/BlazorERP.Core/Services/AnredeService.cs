using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class AnredeService : IModelService<Anrede, int?, AnredeFilter>, ITranslationCode
{
    private readonly ÜbersetzungService _übersetzungService;

    public AnredeService(ÜbersetzungService übersetzungService)
    {
        _übersetzungService = übersetzungService;
    }

    public async Task CreateAsync(Anrede input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO ANREDEN
            (
                NAME,
                LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT
            )
            VALUES
            (
                @NAME,
                @LETZTER_BEARBEITER,
                @ZULETZT_GEAENDERT
            ) RETURNING ANREDE_ID;
            """;



        input.AnredeId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

        foreach (var item in input.Übersetzungen)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.AnredeId;

            await _übersetzungService.CreateAsync(item, dbController, cancellationToken);
        }

    }

    public Task DeleteAsync(Anrede input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE * FROM ANREDEN WHERE ANREDE_ID = @ANREDE_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public async Task<Anrede?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return null;
        }

        string sql = "SELECT * FROM ANREDEN WHERE ANREDE_ID = @ANREDE_ID";

        var result = await dbController.GetFirstAsync<Anrede>(sql, new
        {
            ANREDE_ID = identifier
        }, cancellationToken);

        if (result is not null)
        {
            result.Übersetzungen = await _übersetzungService.GetAsync(GetTranslationCode(), result.AnredeId, dbController, cancellationToken);
        }

        return result;
    }

    public async Task<List<Anrede>> GetAsync(AnredeFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                * 
            FROM ANREDEN 
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY ANREDE_ID DESC
        """;

        var results = await dbController.SelectDataAsync<Anrede>(sql, filter.GetParameters(), cancellationToken);
        if (results.Count > 0)
        {
            var anredeIds = results.Select(x => x.AnredeId).ToArray();
            var übersetzungen = await _übersetzungService.GetAsync(GetTranslationCode(), anredeIds, dbController, cancellationToken);
            foreach (var item in results)
            {
                item.Übersetzungen = übersetzungen.Where(x => x.ParentId == item.AnredeId).ToList();
            }
        }

        return results;
    }

 

    public string GetFilterWhere(AnredeFilter filter)
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

    public Task<int> GetTotalAsync(AnredeFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM ANREDEN
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public string GetTranslationCode() => "ANREDE";

    public async Task UpdateAsync(Anrede input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE ANREDEN SET 
                NAME = @NAME,
                LETZTER_BEARBEITER = @LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT = @ZULETZT_GEAENDERT
            WHERE
                ANREDE_ID = @ANREDE_ID
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);


        await _übersetzungService.ClearAsync(GetTranslationCode(), input.AnredeId, dbController, cancellationToken);
        foreach (var item in input.Übersetzungen)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.AnredeId;

            await _übersetzungService.CreateAsync(item, dbController, cancellationToken);
        }
    }
}
