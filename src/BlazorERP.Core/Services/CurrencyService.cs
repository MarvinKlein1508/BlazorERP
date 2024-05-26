using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class CurrencyService : IModelService<Currency, string?, CurrencyFilter>
{
    public Task CreateAsync(Currency input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO CURRENCIES
            (
                CODE, 
                RATE, 
                EXCHANGE_RATE, 
                MUST_ROUND, 
                DECIMAL_PLACES, 
                RATE_DATE, 
                LAST_MODIFIED_BY,
                LAST_MODIFIED
            )
            VALUES
            (
                @CODE, 
                @RATE, 
                @EXCHANGE_RATE, 
                @MUST_ROUND, 
                @DECIMAL_PLACES, 
                @RATE_DATE, 
                @LAST_MODIFIED_BY,
                @LAST_MODIFIED
            );
            """;

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public Task DeleteAsync(Currency input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM CURRENCIES WHERE CODE = @CODE";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
    public static Task<List<Currency>> GetAsync(IDbController dbController)
    {
        string sql = "SELECT * FROM CURRENCIES";

        return dbController.SelectDataAsync<Currency>(sql);

    }
    public Task<Currency?> GetAsync(string? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return Task.FromResult<Currency?>(null);
        }

        string sql =
            """
            SELECT 
                C.*,
                U.DISPLAY_NAME AS BEARBEITER_NAME
            FROM CURRENCIES C
            LEFT JOIN USERS U ON (U.USER_ID = C.LAST_MODIFIED_BY)
            WHERE CODE = @CODE
            """;

        return dbController.GetFirstAsync<Currency>(sql, new
        {
            CODE = identifier
        }, cancellationToken);
    }

    public Task<List<Currency>> GetAsync(CurrencyFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                C.*,
                U.DISPLAY_NAME AS BEARBEITER_NAME
            FROM CURRENCIES C
            LEFT JOIN USERS U ON (U.USER_ID = C.LAST_MODIFIED_BY)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY CODE DESC
        """;

        return dbController.SelectDataAsync<Currency>(sql, filter.GetParameters(), cancellationToken);

    }



    public string GetFilterWhere(CurrencyFilter filter)
    {
        StringBuilder sb = new();

        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@" AND 
(
        UPPER(CODE) LIKE @SEARCH_PHRASE
)");
        }



        string sql = sb.ToString();
        return sql;
    }

    public Task<int> GetTotalAsync(CurrencyFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM CURRENCIES
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public Task UpdateAsync(Currency input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE CURRENCIES SET 
                RATE = @RATE, 
                EXCHANGE_RATE = @EXCHANGE_RATE, 
                MUST_ROUND = @MUST_ROUND, 
                DECIMAL_PLACES = @DECIMAL_PLACES, 
                RATE_DATE = @RATE_DATE, 
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY,
                LAST_MODIFIED = @LAST_MODIFIED
            WHERE
                CODE = @CODE
            """;


        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public Task<List<Currency>> GetAsync(string?[] identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
