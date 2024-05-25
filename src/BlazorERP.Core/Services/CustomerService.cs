using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BlazorERP.Core.Services;

public class CustomerService : IModelService<Customer, string?, CustomerFilter>
{
    private readonly AddressService _addressService;

    public CustomerService(AddressService addressService)
    {
        _addressService = addressService;
    }
    public async Task CreateAsync(Customer input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO CUSTOMERS
            (
                CUSTOMER_NUMBER,
                COMPANY,
                NAME1,
                NAME2,
                CREATION_DATE,
                LANGUAGE_ID,
                COUNTRY_ID,
                STREET,
                POSTAL_CODE,
                CITY,
                PHONE_NUMBER,
                MOBILE_NUMBER,
                FAX_NUMBER,
                EMAIL,
                WEBSITE,
                NOTE,
                SALUTATION_ID,
                PAYMENT_CONDITION_ID,
                DELIVERY_CONDITION_ID,
                VAT_IDENTIFICATION_NUMBER,
                CREDIT_LIMIT,
                IBAN,
                BIC,
                IS_BLOCKED,
                NEUTRAL_SHIPPING,
                CURRENCY_CODE,
                LAST_MODIFIED_BY,
                LAST_MODIFIED
            )
            VALUES
            (
                @CUSTOMER_NUMBER,
                @COMPANY,
                @NAME1,
                @NAME2,
                @CREATION_DATE,
                @LANGUAGE_ID,
                @COUNTRY_ID,
                @STREET,
                @POSTAL_CODE,
                @CITY,
                @PHONE_NUMBER,
                @MOBILE_NUMBER,
                @FAX_NUMBER,
                @EMAIL,
                @WEBSITE,
                @NOTE,
                @SALUTATION_ID,
                @PAYMENT_CONDITION_ID,
                @DELIVERY_CONDITION_ID,
                @VAT_IDENTIFICATION_NUMBER,
                @CREDIT_LIMIT,
                @IBAN,
                @BIC,
                @IS_BLOCKED,
                @NEUTRAL_SHIPPING,
                @CURRENCY_CODE,
                @LAST_MODIFIED_BY,
                @LAST_MODIFIED
            ) RETURNING CUSTOMER_NUMBER;
            """;

        input.CustomerNumber = (await dbController.GetFirstAsync<string>(sql, input.GetParameters(), cancellationToken))!;
    }

    public Task DeleteAsync(Customer input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM CUSTOMERS WHERE CUSTOMER_NUMBER = @CUSTOMER_NUMBER";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public async Task<Customer?> GetAsync(string? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return null;
        }

        string sql = "SELECT * FROM CUSTOMERS WHERE CUSTOMER_NUMBER = @CUSTOMER_NUMBER";

        var result = await dbController.GetFirstAsync<Customer>(sql, new
        {
            CUSTOMER_NUMBER = identifier
        }, cancellationToken);

        if (result is not null)
        {
            result.Addresses = await _addressService.GetForCustomersAsync([result.CustomerNumber], dbController, cancellationToken);
        }


        return result;
    }

    public Task<List<Customer>> GetAsync(CustomerFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                * 
            FROM CUSTOMERS 
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY CUSTOMER_NUMBER DESC
        """;

        return dbController.SelectDataAsync<Customer>(sql, filter.GetParameters(), cancellationToken);
    }



    public string GetFilterWhere(CustomerFilter filter)
    {
        StringBuilder sb = new();

        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@" AND 
(
        UPPER(COMPANY) LIKE @SEARCH_PHRASE
    OR  UPPER(NAME1) LIKE @SEARCH_PHRASE
    OR  UPPER(NAME2) LIKE @SEARCH_PHRASE
)");
        }



        string sql = sb.ToString();
        return sql;
    }

    public Task<int> GetTotalAsync(CustomerFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM CUSTOMERS
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public async Task UpdateAsync(Customer input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE CUSTOMERS SET 
                COMPANY = @COMPANY,
                NAME1 = @NAME1,
                NAME2 = @NAME2,
                CREATION_DATE = @CREATION_DATE,
                LANGUAGE_ID = @LANGUAGE_ID,
                COUNTRY_ID = @COUNTRY_ID,
                STREET = @STREET,
                POSTAL_CODE = @POSTAL_CODE,
                CITY = @CITY,
                PHONE_NUMBER = @PHONE_NUMBER,
                MOBILE_NUMBER = @MOBILE_NUMBER,
                FAX_NUMBER = @FAX_NUMBER,
                EMAIL = @EMAIL,
                WEBSITE = @WEBSITE,
                NOTE = @NOTE,
                SALUTATION_ID = @SALUTATION_ID,
                PAYMENT_CONDITION_ID = @PAYMENT_CONDITION_ID,
                DELIVERY_CONDITION_ID = @DELIVERY_CONDITION_ID,
                VAT_IDENTIFICATION_NUMBER = @VAT_IDENTIFICATION_NUMBER,
                CREDIT_LIMIT = @CREDIT_LIMIT,
                IBAN = @IBAN,
                BIC = @BIC,
                IS_BLOCKED = @IS_BLOCKED,
                NEUTRAL_SHIPPING = @NEUTRAL_SHIPPING,
                CURRENCY_CODE = @CURRENCY_CODE,
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY,
                LAST_MODIFIED = @LAST_MODIFIED
            WHERE
                CUSTOMER_NUMBER = @CUSTOMER_NUMBER
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }


    
}
