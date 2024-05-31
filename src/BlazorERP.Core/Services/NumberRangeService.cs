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
            INSERT INTO NUMBER_RANGES
            (
                NAME,
                CUSTOMER_FROM,
                CUSTOMER_TO,
                LAST_MODIFIED_BY,
                LAST_MODIFIED
            )
            VALUES
            (
                @NAME,
                @CUSTOMER_FROM,
                @CUSTOMER_TO,
                @LAST_MODIFIED_BY,
                @LAST_MODIFIED
            ) RETURNING NUMBER_RANGE_ID;
            """;

        input.NumberRangeId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
    }

    public Task DeleteAsync(NumberRange input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM NUMBER_RANGES WHERE NUMBER_RANGE_ID = @NUMBER_RANGE_ID";

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
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName
            FROM NUMBER_RANGES N
            LEFT JOIN USERS UC ON (UC.USER_ID = N.CREATED_BY)
            LEFT JOIN USERS UL ON (UL.USER_ID = N.LAST_MODIFIED_BY)
            WHERE 
                NUMBER_RANGE_ID = @NUMBER_RANGE_ID
            """;

        return dbController.GetFirstAsync<NumberRange>(sql, new
        {
            NUMBER_RANGE_ID = identifier
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
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName
            FROM NUMBER_RANGES N
            LEFT JOIN USERS UC ON (UC.USER_ID = N.CREATED_BY)
            LEFT JOIN USERS UL ON (UL.USER_ID = N.LAST_MODIFIED_BY)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY NUMBER_RANGE_ID DESC
        """;

        return dbController.SelectDataAsync<NumberRange>(sql, filter.GetParameters(), cancellationToken);
    }

    public Task<List<NumberRange>> GetAsync(int?[] identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
            FROM NUMBER_RANGES
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }


    public async Task UpdateAsync(NumberRange input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE NUMBER_RANGES SET 
                NAME = @NAME,
                CUSTOMER_FROM = @CUSTOMER_FROM,
                CUSTOMER_TO = @CUSTOMER_TO,
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY,
                LAST_MODIFIED = @LAST_MODIFIED
            WHERE
                NUMBER_RANGE_ID = @NUMBER_RANGE_ID
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
}
