using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class NumberRangeService : IModelService<NumberRange, int?, NumberRangeFilter>
{
    public async Task CreateAsync(NumberRange input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO NUMMERNKREISE
            (
                NAME,
                KUNDE_VON,
                KUNDE_BIS,
                LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT
            )
            VALUES
            (
                @NAME,
                @KUNDE_VON,
                @KUNDE_BIS,
                @LETZTER_BEARBEITER,
                @ZULETZT_GEAENDERT
            ) RETURNING NUMMERNKREIS_ID;
            """;

        input.NummernkreisId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
    }

    public Task DeleteAsync(NumberRange input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM NUMMERNKREISE WHERE NUMMERNKREIS_ID = @NUMMERNKREIS_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public Task<NumberRange?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return Task.FromResult<NumberRange?>(null);
        }

        string sql =
            """
            SELECT 
                N.*,
                U.ANZEIGENAME AS BEARBEITER_NAME
            FROM NUMMERNKREISE N
            LEFT JOIN USERS U ON (U.USER_ID = N.LETZTER_BEARBEITER)
            WHERE 
                NUMMERNKREIS_ID = @NUMMERNKREIS_ID
            """;

        return dbController.GetFirstAsync<NumberRange>(sql, new
        {
            NUMMERNKREIS_ID = identifier
        }, cancellationToken);
    }

    public Task<List<NumberRange>> GetAsync(NumberRangeFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                N.*,
                U.ANZEIGENAME AS BEARBEITER_NAME
            FROM NUMMERNKREISE N
            LEFT JOIN USERS U ON (U.USER_ID = N.LETZTER_BEARBEITER)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY NUMMERNKREIS_ID DESC
        """;

        return dbController.SelectDataAsync<NumberRange>(sql, filter.GetParameters(), cancellationToken);
    }

 

    public string GetFilterWhere(NumberRangeFilter filter)
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

    public Task<int> GetTotalAsync(NumberRangeFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM NUMMERNKREISE
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }


    public async Task UpdateAsync(NumberRange input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE NUMMERNKREISE SET 
                NAME = @NAME,
                KUNDE_VON = @KUNDE_VON,
                KUNDE_BIS = @KUNDE_BIS,
                LETZTER_BEARBEITER = @LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT = @ZULETZT_GEAENDERT
            WHERE
                NUMMERNKREIS_ID = @NUMMERNKREIS_ID
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
}
