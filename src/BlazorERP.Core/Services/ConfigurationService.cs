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
            INSERT INTO CONFIGURATION
            (
                NAME,
                DEFAULT_LANGUAGE_ID,
                CUSTOMER_DELIVERY_CONDITION_ID,
                CUSTOMER_PAYMENT_CONDITION_ID,
                CUSTOMER_SALUTATION_ID,
                CUSTOMER_CURRENCY_CODE,
                CUSTOMER_COUNTRY_ID,
                CUSTOMER_LANGUAGE_ID,
                CUSTOMER_CREDIT_LIMIT,
                CUSTOMER_NEUTRAL_SHIPPING,
                LAST_MODIFIED_BY,
                LAST_MODIFIED
            )
            VALUES
            (
                @NAME,
                @DEFAULT_LANGUAGE_ID,
                @CUSTOMER_DELIVERY_CONDITION_ID,
                @CUSTOMER_PAYMENT_CONDITION_ID,
                @CUSTOMER_SALUTATION_ID,
                @CUSTOMER_CURRENCY_CODE,
                @CUSTOMER_COUNTRY_ID,
                @CUSTOMER_LANGUAGE_ID,
                @CUSTOMER_CREDIT_LIMIT,
                @CUSTOMER_NEUTRAL_SHIPPING,
                @LAST_MODIFIED_BY,
                @LAST_MODIFIED
            ) RETURNING CONFIGURATION_ID;
            """;

        input.ConfigurationId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
    }

    public Task DeleteAsync(Configuration input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM CONFIGURATION WHERE CONFIGURATION_ID = @CONFIGURATION_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public Task<Configuration?> GetAsync(int? configurationId, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (configurationId is null)
        {
            return Task.FromResult<Configuration?>(null);
        }

        string sql =
            """
            SELECT 
                C.*,
                U.DISPLAY_NAME AS BEARBEITER_NAME
            FROM CONFIGURATION C
            LEFT JOIN USERS U ON (U.USER_ID = C.LAST_MODIFIED_BY)
            WHERE 
                CONFIGURATION_ID = @CONFIGURATION_ID
            """;

        return dbController.GetFirstAsync<Configuration>(sql, new
        {
            CONFIGURATION_ID = configurationId
        }, cancellationToken);
    }

    public Task<List<Configuration>> GetAsync(ConfigurationFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                C.*,
                U.DISPLAY_NAME AS BEARBEITER_NAME
            FROM CONFIGURATION C
            LEFT JOIN USERS U ON (U.USER_ID = C.LAST_MODIFIED_BY)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY CONFIGURATION_ID DESC
        """;

        return dbController.SelectDataAsync<Configuration>(sql, filter.GetParameters(), cancellationToken);
    }

    public Task<List<Configuration>> GetAsync(int?[] identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
            FROM CONFIGURATION
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }


    public async Task UpdateAsync(Configuration input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE CONFIGURATION SET 
                NAME = @NAME,
                DEFAULT_LANGUAGE_ID = @DEFAULT_LANGUAGE_ID,
                CUSTOMER_DELIVERY_CONDITION_ID = @CUSTOMER_DELIVERY_CONDITION_ID,
                CUSTOMER_PAYMENT_CONDITION_ID = @CUSTOMER_PAYMENT_CONDITION_ID,
                CUSTOMER_SALUTATION_ID = @CUSTOMER_SALUTATION_ID,
                CUSTOMER_CURRENCY_CODE = @CUSTOMER_CURRENCY_CODE,
                CUSTOMER_COUNTRY_ID = @CUSTOMER_COUNTRY_ID,
                CUSTOMER_LANGUAGE_ID = @CUSTOMER_LANGUAGE_ID,
                CUSTOMER_CREDIT_LIMIT = @CUSTOMER_CREDIT_LIMIT,
                CUSTOMER_NEUTRAL_SHIPPING = @CUSTOMER_NEUTRAL_SHIPPING,
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY,
                LAST_MODIFIED = @LAST_MODIFIED
            WHERE
                CONFIGURATION_ID = @CONFIGURATION_ID
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
}
