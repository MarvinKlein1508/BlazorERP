using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class CostCenterService : IModelService<CostCenter, int?, CostCenterFilter>
{
    public async Task CreateAsync(CostCenter input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO COST_CENTERS
            (
                COST_CENTER_NUMBER,
                NAME,
                CREATION_DATE,
                LAST_MODIFIED,
                LAST_MODIFIED_BY
            )
            VALUES
            (
                @COST_CENTER_NUMBER,
                @NAME,
                @CREATION_DATE,
                @LAST_MODIFIED,
                @LAST_MODIFIED_BY
            ) RETURNING COST_CENTER_ID;
            """;

        input.CostCenterId =  await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
    }

    public async Task DeleteAsync(CostCenter input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM COST_CENTERS WHERE COST_CENTER_ID = @COST_CENTER_ID";

        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
    public static async Task<List<CostCenter>> GetAsync(IDbController dbController)
    {
        string sql = "SELECT * FROM COST_CENTERS";

        var results = await dbController.SelectDataAsync<CostCenter>(sql);

        return results;
    }
    public async Task<CostCenter?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return null;
        }

        string sql =
            """
            SELECT 
                C.*,
                U.DISPLAY_NAME AS BEARBEITER_NAME
            FROM COST_CENTERS C
            LEFT JOIN USERS U ON (U.USER_ID = C.LAST_MODIFIED_BY)
            WHERE 
                COST_CENTER_ID = @COST_CENTER_ID
            """;

        var result = await dbController.GetFirstAsync<CostCenter>(sql, new
        {
            COST_CENTER_ID = identifier
        }, cancellationToken);

        return result;  
    }

    public Task<List<CostCenter>> GetAsync(CostCenterFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                C.*,
                U.DISPLAY_NAME AS BEARBEITER_NAME
            FROM COST_CENTERS C
            LEFT JOIN USERS U ON (U.USER_ID = C.LAST_MODIFIED_BY)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY COST_CENTER_ID DESC
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

    public async Task<int> GetTotalAsync(CostCenterFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM COST_CENTERS
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        var result = await dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);

        return result;
    }


    public async Task UpdateAsync(CostCenter input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE COST_CENTERS SET 
                COST_CENTER_NUMBER = @COST_CENTER_NUMBER,
                NAME = @NAME,
                CREATION_DATE = @CREATION_DATE,
                LAST_MODIFIED = @LAST_MODIFIED,
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY
            WHERE
                COST_CENTER_ID = @COST_CENTER_ID
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public Task<List<CostCenter>> GetAsync(int?[] identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
