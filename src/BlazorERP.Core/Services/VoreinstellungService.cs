using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class VoreinstellungService : IModelService<Voreinstellung, int?, VoreinstellungFilter>
{
    public async Task CreateAsync(Voreinstellung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO VOREINSTELLUNGEN
            (
                NAME,
                KUNDE_LIEFERBEDINGUNG_ID,
                KUNDE_ZAHLUNGSBEDINGUNG_ID,
                KUNDE_ANREDE_ID,
                KUNDE_WAEHRUNGSCODE,
                KUNDE_LAND_ID,
                KUNDE_KREDITLIMIT,
                KUNDE_NEUTRALER_VERSAND,
                LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT
            )
            VALUES
            (
                @NAME,
                @KUNDE_LIEFERBEDINGUNG_ID,
                @KUNDE_ZAHLUNGSBEDINGUNG_ID,
                @KUNDE_ANREDE_ID,
                @KUNDE_WAEHRUNGSCODE,
                @KUNDE_LAND_ID,
                @KUNDE_KREDITLIMIT,
                @KUNDE_NEUTRALER_VERSAND,
                @LETZTER_BEARBEITER,
                @ZULETZT_GEAENDERT
            ) RETURNING VOREINSTELLUNG_ID;
            """;

        input.VoreinstellungId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
    }

    public Task DeleteAsync(Voreinstellung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM VOREINSTELLUNGEN WHERE VOREINSTELLUNG_ID = @VOREINSTELLUNG_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public Task<Voreinstellung?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return Task.FromResult<Voreinstellung?>(null);
        }

        string sql =
            """
            SELECT 
                V.*,
                U.ANZEIGENAME AS BEARBEITER_NAME
            FROM VOREINSTELLUNGEN V
            LEFT JOIN USERS U ON (U.USER_ID = V.LETZTER_BEARBEITER)
            WHERE 
                VOREINSTELLUNG_ID = @VOREINSTELLUNG_ID
            """;

        return dbController.GetFirstAsync<Voreinstellung>(sql, new
        {
            VOREINSTELLUNG_ID = identifier
        }, cancellationToken);
    }

    public Task<List<Voreinstellung>> GetAsync(VoreinstellungFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                V.*,
                U.ANZEIGENAME AS BEARBEITER_NAME
            FROM VOREINSTELLUNGEN V
            LEFT JOIN USERS U ON (U.USER_ID = V.LETZTER_BEARBEITER)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY VOREINSTELLUNG_ID DESC
        """;

        return dbController.SelectDataAsync<Voreinstellung>(sql, filter.GetParameters(), cancellationToken);
    }

 

    public string GetFilterWhere(VoreinstellungFilter filter)
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

    public Task<int> GetTotalAsync(VoreinstellungFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM VOREINSTELLUNGEN
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }


    public async Task UpdateAsync(Voreinstellung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE VOREINSTELLUNGEN SET 
                NAME = @NAME,
                KUNDE_LIEFERBEDINGUNG_ID = @KUNDE_LIEFERBEDINGUNG_ID,
                KUNDE_ZAHLUNGSBEDINGUNG_ID = @KUNDE_ZAHLUNGSBEDINGUNG_ID,
                KUNDE_ANREDE_ID = @KUNDE_ANREDE_ID,
                KUNDE_WAEHRUNGSCODE = @KUNDE_WAEHRUNGSCODE,
                KUNDE_LAND_ID = @KUNDE_LAND_ID,
                KUNDE_KREDITLIMIT = @KUNDE_KREDITLIMIT,
                KUNDE_NEUTRALER_VERSAND = @KUNDE_NEUTRALER_VERSAND,
                LETZTER_BEARBEITER = @LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT = @ZULETZT_GEAENDERT
            WHERE
                VOREINSTELLUNG_ID = @VOREINSTELLUNG_ID
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
}
