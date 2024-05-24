using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class CostCenterService : IModelService<CostCenter, int?, CostCenterFilter>
{
    public Task CreateAsync(CostCenter input, IDbController dbController, CancellationToken cancellationToken = default)
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

    public Task DeleteAsync(CostCenter input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM KOSTENSTELLEN WHERE NUMMER = @NUMMER";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
    public static Task<List<CostCenter>> GetAsync(IDbController dbController)
    {
        string sql = "SELECT * FROM KOSTENSTELLEN";

        return dbController.SelectDataAsync<CostCenter>(sql);
    }
    public Task<CostCenter?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return Task.FromResult<CostCenter?>(null);
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

        return dbController.GetFirstAsync<CostCenter>(sql, new
        {
            NUMMER = identifier
        }, cancellationToken);
    }

    public Task<List<CostCenter>> GetAsync(CostCenterFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
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

        return dbController.SelectDataAsync<CostCenter>(sql, filter.GetParameters(), cancellationToken);
    }

 

    public string GetFilterWhere(CostCenterFilter filter)
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

    public Task<int> GetTotalAsync(CostCenterFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
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


    public async Task UpdateAsync(CostCenter input, IDbController dbController, CancellationToken cancellationToken = default)
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
