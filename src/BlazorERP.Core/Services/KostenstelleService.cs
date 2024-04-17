using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class KostenstelleService : IModelService<Kostenstelle, int?, KostenstelleFilter>
{
    public Task CreateAsync(Kostenstelle input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO KOSTENSTELLEN
            (
                NUMMER,
                NAME,
                LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT
            )
            VALUES
            (
                @NUMMER,
                @NAME,
                @LETZTER_BEARBEITER,
                @ZULETZT_GEAENDERT
            );
            """;

        return dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
    }

    public Task DeleteAsync(Kostenstelle input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM KOSTENSTELLEN WHERE NUMMER = @NUMMER";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public Task<Kostenstelle?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return Task.FromResult<Kostenstelle?>(null);
        }

        string sql =
            """
            SELECT 
                K.*,
                U.ANZEIGENAME AS BEARBEITER_NAME
            FROM KOSTENSTELLEN K
            LEFT JOIN USERS U ON (U.USER_ID = K.LETZTER_BEARBEITER)
            WHERE 
                NUMMER = @NUMMER
            """;

        return dbController.GetFirstAsync<Kostenstelle>(sql, new
        {
            NUMMER = identifier
        }, cancellationToken);
    }

    public Task<List<Kostenstelle>> GetAsync(KostenstelleFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                K.*,
                U.ANZEIGENAME AS BEARBEITER_NAME
            FROM KOSTENSTELLEN K
            LEFT JOIN USERS U ON (U.USER_ID = K.LETZTER_BEARBEITER)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY NUMMER DESC
        """;

        return dbController.SelectDataAsync<Kostenstelle>(sql, filter.GetParameters(), cancellationToken);
    }

 

    public string GetFilterWhere(KostenstelleFilter filter)
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

    public Task<int> GetTotalAsync(KostenstelleFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM KOSTENSTELLEN
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }


    public async Task UpdateAsync(Kostenstelle input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE KOSTENSTELLEN SET 
                NAME = @NAME,
                LETZTER_BEARBEITER = @LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT = @ZULETZT_GEAENDERT
            WHERE
                NUMMER = @NUMMER
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
}
