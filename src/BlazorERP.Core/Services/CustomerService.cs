using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BlazorERP.Core.Services;

public class CustomerService : IModelService<Customer, string?, CustomerFilter>
{
    private readonly CountryService _countryService;
    private readonly AddressService _addressService;

    public CustomerService(CountryService countryService, AddressService addressService)
    {
        _countryService = countryService;
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
                CREATED_AT,
                CREATED_BY,
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
                @CREATED_AT,
                @CREATED_BY,
                @LAST_MODIFIED_BY,
                @LAST_MODIFIED
            ) RETURNING CUSTOMER_NUMBER;
            """;

        input.CustomerNumber = (await dbController.GetFirstAsync<string>(sql, input.GetParameters(), cancellationToken))!;

        await _addressService.AssignCustomerAsync(input, dbController, cancellationToken);


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

        string sql =
            """
            SELECT 
                C.*,
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName
            FROM CUSTOMERS C 
            LEFT JOIN USERS UC ON (UC.USER_ID = C.CREATED_BY)
            LEFT JOIN USERS UL ON (UL.USER_ID = C.LAST_MODIFIED_BY)
            WHERE 
                CUSTOMER_NUMBER = @CUSTOMER_NUMBER";
            """;

        var result = await dbController.GetFirstAsync<Customer>(sql, new
        {
            CUSTOMER_NUMBER = identifier
        }, cancellationToken);

        if (result is not null)
        {
            result.Country = await _countryService.GetAsync(result.CountryId, dbController, cancellationToken);
            result.Addresses = await _addressService.GetForCustomersAsync([result.CustomerNumber], dbController, cancellationToken);
        }


        return result;
    }

    public Task<List<Customer>> GetAsync(string?[] identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public async Task<List<Customer>> GetAsync(CustomerFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                C.*,
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName
            FROM CUSTOMERS C
            LEFT JOIN USERS UC ON (UC.USER_ID = C.CREATED_BY)
            LEFT JOIN USERS UL ON (UL.USER_ID = C.LAST_MODIFIED_BY)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY CUSTOMER_NUMBER DESC
        """;

        var results = await dbController.SelectDataAsync<Customer>(sql, filter.GetParameters(), cancellationToken);

        if (results.Count > 0)
        {
            var customerNumbers = results.Select(x => x.CustomerNumber).Distinct().ToArray();
            var countryIds = results.Select(x => x.CountryId).Distinct().ToArray();

            var addresses = await _addressService.GetForCustomersAsync(customerNumbers, dbController, cancellationToken);
            var countries = await _countryService.GetAsync(countryIds, dbController, cancellationToken);

            foreach (var item in results)
            {
                item.Country = countries.FirstOrDefault(x => x.CountryId == item.CountryId);
            }

        }

        return results;
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

    public async Task<int> GetTotalAsync(CustomerFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
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


        var result = await dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);

        return result;
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
                CREATED_AT = @CREATED_AT,
                CREATED_BY = @CREATED_BY,
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY,
                LAST_MODIFIED = @LAST_MODIFIED
            WHERE
                CUSTOMER_NUMBER = @CUSTOMER_NUMBER
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);


        await _addressService.CleanCustomerAssignmentAsync(input, dbController, cancellationToken);
        await _addressService.AssignCustomerAsync(input, dbController, cancellationToken);
    }



}
