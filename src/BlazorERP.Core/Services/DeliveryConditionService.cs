using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class DeliveryConditionService : IModelService<DeliveryCondition, int?, DeliveryConditionFilter>, ITranslationCode
{
    private readonly TranslationService _übersetzungService;

    public DeliveryConditionService(TranslationService übersetzungService)
    {
        _übersetzungService = übersetzungService;
    }

    public async Task CreateAsync(DeliveryCondition input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO LIEFERBEDINGUNGEN
            (
                NAME,
                VERSAND_MIT_SPEDITION,
                IST_ABHOLUNG,
                VERFUEGBAR_KUNDE,
                VERFUEGBAR_LIEFERANT,
                IST_AKTIV,
                LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT
            )
            VALUES
            (
                @NAME,
                @VERSAND_MIT_SPEDITION,
                @IST_ABHOLUNG,
                @VERFUEGBAR_KUNDE,
                @VERFUEGBAR_LIEFERANT,
                @IST_AKTIV,
                @LETZTER_BEARBEITER,
                @ZULETZT_GEAENDERT
            ) RETURNING LIEFERBEDINGUNG_ID;
            """;



        input.LieferbedingungId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

        foreach (var item in input.Übersetzungen)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.LieferbedingungId;

            await _übersetzungService.CreateAsync(item, dbController, cancellationToken);
        }

    }

    public Task DeleteAsync(DeliveryCondition input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM LIEFERBEDINGUNGEN WHERE LIEFERBEDINGUNG_ID = @LIEFERBEDINGUNG_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public static async Task<List<DeliveryCondition>> GetAsync(IDbController dbController)
    {
        string sql = "SELECT * FROM LIEFERBEDINGUNGEN";

        var results = await dbController.SelectDataAsync<DeliveryCondition>(sql);


        var übersetzungen = await TranslationService.GetAsync(GetTranslationCode(), dbController);

        foreach (var item in results)
        {
            item.Übersetzungen = übersetzungen.Where(x => x.ParentId == item.LieferbedingungId).ToList();
        }

        return results;
    }
    public async Task<DeliveryCondition?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return null;
        }

        string sql =
            """
            SELECT 
                L.*,
                U.ANZEIGENAME AS BEARBEITER_NAME
            FROM LIEFERBEDINGUNGEN L
            LEFT JOIN USERS U ON (U.USER_ID = L.LETZTER_BEARBEITER)
            WHERE 
                LIEFERBEDINGUNG_ID = @LIEFERBEDINGUNG_ID
            """;

        var result = await dbController.GetFirstAsync<DeliveryCondition>(sql, new
        {
            LIEFERBEDINGUNG_ID = identifier
        }, cancellationToken);

        if (result is not null)
        {
            result.Übersetzungen = await _übersetzungService.GetAsync(GetTranslationCode(), result.LieferbedingungId, dbController, cancellationToken);
        }

        return result;
    }

    public async Task<List<DeliveryCondition>> GetAsync(DeliveryConditionFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                L.*,
                U.ANZEIGENAME AS BEARBEITER_NAME 
            FROM LIEFERBEDINGUNGEN L 
            LEFT JOIN USERS U ON (U.USER_ID = L.LETZTER_BEARBEITER)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY LIEFERBEDINGUNG_ID DESC
        """;

        var results = await dbController.SelectDataAsync<DeliveryCondition>(sql, filter.GetParameters(), cancellationToken);
        if (results.Count > 0)
        {
            var anredeIds = results.Select(x => x.LieferbedingungId).ToArray();
            var übersetzungen = await _übersetzungService.GetAsync(GetTranslationCode(), anredeIds, dbController, cancellationToken);
            foreach (var item in results)
            {
                item.Übersetzungen = übersetzungen.Where(x => x.ParentId == item.LieferbedingungId).ToList();
            }
        }

        return results;
    }

 

    public string GetFilterWhere(DeliveryConditionFilter filter)
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

    public Task<int> GetTotalAsync(DeliveryConditionFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM LIEFERBEDINGUNGEN
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public static string GetTranslationCode() => "LIEFERBEDINGUNG";

    public async Task UpdateAsync(DeliveryCondition input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE LIEFERBEDINGUNGEN SET 
                NAME = @NAME,
                VERSAND_MIT_SPEDITION = @VERSAND_MIT_SPEDITION,
                IST_ABHOLUNG = @IST_ABHOLUNG,
                VERFUEGBAR_KUNDE = @VERFUEGBAR_KUNDE,
                VERFUEGBAR_LIEFERANT = @VERFUEGBAR_LIEFERANT,
                IST_AKTIV = @IST_AKTIV,
                LETZTER_BEARBEITER = @LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT = @ZULETZT_GEAENDERT            
            WHERE
                LIEFERBEDINGUNG_ID = @LIEFERBEDINGUNG_ID
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);


        await _übersetzungService.ClearAsync(GetTranslationCode(), input.LieferbedingungId, dbController, cancellationToken);
        foreach (var item in input.Übersetzungen)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.LieferbedingungId;

            await _übersetzungService.CreateAsync(item, dbController, cancellationToken);
        }
    }
}
