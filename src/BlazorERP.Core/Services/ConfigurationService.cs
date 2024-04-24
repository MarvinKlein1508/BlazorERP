using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class ConfigurationService : IModelService<Configuration, int?, ConfigurationFilter>
{
    public async Task CreateAsync(Configuration input, IDbController dbController, CancellationToken cancellationToken = default)
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
                KUNDE_SPRACH_ID,
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
                @KUNDE_SPRACH_ID,
                @KUNDE_KREDITLIMIT,
                @KUNDE_NEUTRALER_VERSAND,
                @LETZTER_BEARBEITER,
                @ZULETZT_GEAENDERT
            ) RETURNING VOREINSTELLUNG_ID;
            """;

        input.ConfigurationId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
    }

    public Task DeleteAsync(Configuration input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM VOREINSTELLUNGEN WHERE VOREINSTELLUNG_ID = @VOREINSTELLUNG_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public Task<Configuration?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return Task.FromResult<Configuration?>(null);
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

        return dbController.GetFirstAsync<Configuration>(sql, new
        {
            VOREINSTELLUNG_ID = identifier
        }, cancellationToken);
    }

    public Task<List<Configuration>> GetAsync(ConfigurationFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
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

        return dbController.SelectDataAsync<Configuration>(sql, filter.GetParameters(), cancellationToken);
    }

 

    public string GetFilterWhere(ConfigurationFilter filter)
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

    public Task<int> GetTotalAsync(ConfigurationFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
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


    public async Task UpdateAsync(Configuration input, IDbController dbController, CancellationToken cancellationToken = default)
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
                KUNDE_SPRACH_ID = @KUNDE_SPRACH_ID,
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
