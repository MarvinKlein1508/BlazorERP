using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class LandService : IModelService<Land, int?, LandFilter>, ITranslationCode
{
    private readonly ÜbersetzungService _übersetzungService;

    public LandService(ÜbersetzungService übersetzungService)
    {
        _übersetzungService = übersetzungService;
    }

    public async Task CreateAsync(Land input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO LAENDER
            (
                NAME,
                ISO2,
                ISO3,
                VORWAHL,
                LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT
            )
            VALUES
            (
                @NAME,
                @ISO2,
                @ISO3,
                @VORWAHL,
                @LETZTER_BEARBEITER,
                @ZULETZT_GEAENDERT
            ) RETURNING LAND_ID;
            """;



        input.LandId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

        foreach (var item in input.Übersetzungen)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.LandId;

            await _übersetzungService.CreateAsync(item, dbController, cancellationToken);
        }

    }

    public Task DeleteAsync(Land input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM LAENDER WHERE LAND_ID = @LAND_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public static async Task<List<Land>> GetAsync(IDbController dbController)
    {
        string sql = "SELECT * FROM LAENDER";

        var results = await dbController.SelectDataAsync<Land>(sql);


        var übersetzungen = await ÜbersetzungService.GetAsync(GetTranslationCode(), dbController);

        foreach (var item in results)
        {
            item.Übersetzungen = übersetzungen.Where(x => x.ParentId == item.LandId).ToList();
        }

        return results;
    }
    public async Task<Land?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return null;
        }

        string sql = "SELECT * FROM LAENDER WHERE LAND_ID = @LAND_ID";

        var result = await dbController.GetFirstAsync<Land>(sql, new
        {
            LAND_ID = identifier
        }, cancellationToken);

        if (result is not null)
        {
            result.Übersetzungen = await _übersetzungService.GetAsync(GetTranslationCode(), result.LandId, dbController, cancellationToken);
        }

        return result;
    }

    public async Task<List<Land>> GetAsync(LandFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                * 
            FROM LAENDER 
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY LAND_ID DESC
        """;

        var results = await dbController.SelectDataAsync<Land>(sql, filter.GetParameters(), cancellationToken);
        if (results.Count > 0)
        {
            var landIds = results.Select(x => x.LandId).ToArray();
            var übersetzungen = await _übersetzungService.GetAsync(GetTranslationCode(), landIds, dbController, cancellationToken);
            foreach (var item in results)
            {
                item.Übersetzungen = übersetzungen.Where(x => x.ParentId == item.LandId).ToList();
            }
        }

        return results;
    }

    public string GetFilterWhere(LandFilter filter)
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

    public Task<int> GetTotalAsync(LandFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM LAENDER
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public static string GetTranslationCode() => "LAND";

    public async Task UpdateAsync(Land input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE LAENDER SET 
                NAME = @NAME,
                ISO2 = @ISO2,
                ISO3 = @ISO3,
                VORWAHL = @VORWAHL,
                LETZTER_BEARBEITER = @LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT = @ZULETZT_GEAENDERT
            WHERE
                LAND_ID = @LAND_ID
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);


        await _übersetzungService.ClearAsync(GetTranslationCode(), input.LandId, dbController, cancellationToken);
        foreach (var item in input.Übersetzungen)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.LandId;

            await _übersetzungService.CreateAsync(item, dbController, cancellationToken);
        }
    }
}
