using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class ZahlungsbedingungService : IModelService<Zahlungsbedingung, int?, ZahlungsbedingungFilter>, ITranslationCode
{
    private readonly ÜbersetzungService _übersetzungService;

    public ZahlungsbedingungService(ÜbersetzungService übersetzungService)
    {
        _übersetzungService = übersetzungService;
    }

    public async Task CreateAsync(Zahlungsbedingung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO ZAHLUNGSBEDINGUNGEN
            (
                NAME,
                NETTOTAGE,
                SKONTO1_TAGE,
                SKONTO1_PROZENT,
                SKONTO2_TAGE,
                SKONTO2_PROZENT,
                IST_VORKASSE,
                IST_BARZAHLUNG,
                IST_ABBUCHUNG,
                IST_RECHNUNG,
                IST_AKTIV,
                VERFUEGBAR_KUNDE,
                VERFUEGBAR_LIEFERANT,
                LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT
            )
            VALUES
            (
                @NAME,
                @NETTOTAGE,
                @SKONTO1_TAGE,
                @SKONTO1_PROZENT,
                @SKONTO2_TAGE,
                @SKONTO2_PROZENT,
                @IST_VORKASSE,
                @IST_BARZAHLUNG,
                @IST_ABBUCHUNG,
                @IST_RECHNUNG,
                @IST_AKTIV,
                @VERFUEGBAR_KUNDE,
                @VERFUEGBAR_LIEFERANT,
                @LETZTER_BEARBEITER,
                @ZULETZT_GEAENDERT
            ) RETURNING ZAHLUNGSBEDINGUNG_ID;
            """;



        input.ZahlungsbedingungId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

        foreach (var item in input.Übersetzungen)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.ZahlungsbedingungId;

            await _übersetzungService.CreateAsync(item, dbController, cancellationToken);
        }

    }

    public Task DeleteAsync(Zahlungsbedingung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM ZAHLUNGSBEDINGUNGEN WHERE ZAHLUNGSBEDINGUNG_ID = @ZAHLUNGSBEDINGUNG_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public async Task<Zahlungsbedingung?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return null;
        }

        string sql =
            """
            SELECT 
                Z.*,
                U.ANZEIGENAME AS BEARBEITER_NAME
            FROM ZAHLUNGSBEDINGUNGEN Z
            LEFT JOIN USERS U ON (U.USER_ID = Z.LETZTER_BEARBEITER)
            WHERE 
                ZAHLUNGSBEDINGUNG_ID = @ZAHLUNGSBEDINGUNG_ID
            """;

        var result = await dbController.GetFirstAsync<Zahlungsbedingung>(sql, new
        {
            ZAHLUNGSBEDINGUNG_ID = identifier
        }, cancellationToken);

        if (result is not null)
        {
            result.Übersetzungen = await _übersetzungService.GetAsync(GetTranslationCode(), result.ZahlungsbedingungId, dbController, cancellationToken);
        }

        return result;
    }

    public async Task<List<Zahlungsbedingung>> GetAsync(ZahlungsbedingungFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                Z.*,
                U.ANZEIGENAME AS BEARBEITER_NAME 
            FROM ZAHLUNGSBEDINGUNGEN Z 
            LEFT JOIN USERS U ON (U.USER_ID = Z.LETZTER_BEARBEITER)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY ZAHLUNGSBEDINGUNG_ID DESC
        """;

        var results = await dbController.SelectDataAsync<Zahlungsbedingung>(sql, filter.GetParameters(), cancellationToken);
        if (results.Count > 0)
        {
            var anredeIds = results.Select(x => x.ZahlungsbedingungId).ToArray();
            var übersetzungen = await _übersetzungService.GetAsync(GetTranslationCode(), anredeIds, dbController, cancellationToken);
            foreach (var item in results)
            {
                item.Übersetzungen = übersetzungen.Where(x => x.ParentId == item.ZahlungsbedingungId).ToList();
            }
        }

        return results;
    }

 

    public string GetFilterWhere(ZahlungsbedingungFilter filter)
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

    public Task<int> GetTotalAsync(ZahlungsbedingungFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM ZAHLUNGSBEDINGUNGEN
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public string GetTranslationCode() => "ZAHLUNGSBEDINGUNG";

    public async Task UpdateAsync(Zahlungsbedingung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE ZAHLUNGSBEDINGUNGEN SET 
                NAME = @NAME,
                NETTOTAGE = @NETTOTAGE,
                SKONTO1_TAGE = @SKONTO1_TAGE,
                SKONTO1_PROZENT = @SKONTO1_PROZENT,
                SKONTO2_TAGE = @SKONTO2_TAGE,
                SKONTO2_PROZENT = @SKONTO2_PROZENT,
                IST_VORKASSE = @IST_VORKASSE,
                IST_BARZAHLUNG = @IST_BARZAHLUNG,
                IST_ABBUCHUNG = @IST_ABBUCHUNG,
                IST_RECHNUNG = @IST_RECHNUNG,
                IST_AKTIV = @IST_AKTIV,
                VERFUEGBAR_KUNDE = @VERFUEGBAR_KUNDE,
                VERFUEGBAR_LIEFERANT = @VERFUEGBAR_LIEFERANT,
                LETZTER_BEARBEITER = @LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT = @ZULETZT_GEAENDERT            
            WHERE
                ZAHLUNGSBEDINGUNG_ID = @ZAHLUNGSBEDINGUNG_ID
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);


        await _übersetzungService.ClearAsync(GetTranslationCode(), input.ZahlungsbedingungId, dbController, cancellationToken);
        foreach (var item in input.Übersetzungen)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.ZahlungsbedingungId;

            await _übersetzungService.CreateAsync(item, dbController, cancellationToken);
        }
    }
}
