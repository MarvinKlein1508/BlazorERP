using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Net.Http.Headers;
using System.Text;

namespace BlazorERP.Core.Services;

public class AddressService : IModelService<Address, int?, AddressFilter>
{
    private readonly CountryService _countryService;

    public AddressService(CountryService countryService)
    {
        _countryService = countryService;
    }
    public async Task CreateAsync(Address input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO ADDRESSES
            (              
                COMPANY,
                NAME1,
                NAME2,
                STREET,
                COUNTRY_ID,
                LANGUAGE_ID,
                POSTAL_CODE,
                CITY,
                PHONE_NUMBER,
                MOBILE_NUMBER,
                FAX_NUMBER,
                EMAIL,
                CONTACT_PERSON_ID,
                VAT_IDENTIFICATION_NUMBER,
                NOTE,
                LAST_MODIFIED_BY,
                LAST_MODIFIED
            )
            VALUES
            (
                @COMPANY,
                @NAME1,
                @NAME2,
                @STREET,
                @COUNTRY_ID,
                @LANGUAGE_ID,
                @POSTAL_CODE,
                @CITY,
                @PHONE_NUMBER,
                @MOBILE_NUMBER,
                @FAX_NUMBER,
                @EMAIL,
                @CONTACT_PERSON_ID,
                @VAT_IDENTIFICATION_NUMBER,
                @NOTE,
                @LAST_MODIFIED_BY,
                @LAST_MODIFIED
            ) RETURNING ADDRESS_ID;
            """;



        input.AddressId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
    }

    public Task DeleteAsync(Address input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM ADDRESSES WHERE ADDRESS_ID = @ADDRESS_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public async Task<Address?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return null;
        }

        string sql =
            """
            SELECT 
                A.*,
                U.DISPLAY_NAME AS BEARBEITER_NAME
            FROM ADDRESSES A 
            LEFT JOIN USERS U ON (U.USER_ID = A.LAST_MODIFIED_BY)
            WHERE 
                ADDRESS_ID = @ADDRESS_ID
            """;

        var result = await dbController.GetFirstAsync<Address>(sql, new
        {
            ADDRESS_ID = identifier
        }, cancellationToken);

        if (result is not null)
        {
            result.Country = await _countryService.GetAsync(result.CountryId, dbController, cancellationToken);
        }

        return result;
    }

    public async Task<List<Address>> GetAsync(AddressFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                A.*,
                U.DISPLAY_NAME AS BEARBEITER_NAME
        FROM ADDRESSES A
        LEFT JOIN USERS U ON (U.USER_ID = A.LAST_MODIFIED_BY)
        WHERE 1 = 1
            {GetFilterWhere(filter)}
        ORDER BY ADDRESS_ID DESC
        """;

        var results = await dbController.SelectDataAsync<Address>(sql, filter.GetParameters(), cancellationToken);

        if (results.Count > 0)
        {
            var countryIds = results.Select(x => x.CountryId).ToArray();
            var countries = await _countryService.GetAsync(countryIds, dbController, cancellationToken);

            foreach (var item in results)
            {
                item.Country = countries.FirstOrDefault(x => x.CountryId == item.CountryId);
            }
        }

        return results;
    }

    public string GetFilterWhere(AddressFilter filter)
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

        if (filter.Blocked.Count > 0)
        {
            sb.AppendLine($"AND ADDRESS_ID NOT IN ({filter.GetBlockedQuery()})");
        }


        string sql = sb.ToString();
        return sql;
    }

    public Task<int> GetTotalAsync(AddressFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM ADDRESSES
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public Task UpdateAsync(Address input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            UPDATE ADDRESSES SET            
                COMPANY = @COMPANY,
                NAME1 = @NAME1,
                NAME2 = @NAME2,
                STREET = @STREET,
                COUNTRY_ID = @COUNTRY_ID,
                LANGUAGE_ID = @LANGUAGE_ID,
                POSTAL_CODE = @POSTAL_CODE,
                CITY = @CITY,
                PHONE_NUMBER = @PHONE_NUMBER,
                MOBILE_NUMBER = @MOBILE_NUMBER,
                FAX_NUMBER = @FAX_NUMBER,
                EMAIL = @EMAIL,
                CONTACT_PERSON_ID = @CONTACT_PERSON_ID,
                VAT_IDENTIFICATION_NUMBER = @VAT_IDENTIFICATION_NUMBER,
                NOTE = @NOTE,
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY,
                LAST_MODIFIED = @LAST_MODIFIED
            WHERE
                ADDRESS_ID = @ADDRESS_ID
            """;

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public async Task<List<Address>> GetForCustomersAsync(string[] customerNumbers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (customerNumbers.Length is 0)
        {
            return [];
        }

        var parameters = new Dictionary<string, object?>();

        List<string> parameterNames = [];

        for (int i = 0; i < customerNumbers.Length; i++)
        {
            string parameterName = $"@CUSTOMER_NUMBER{i}";
            parameterNames.Add(parameterName);
            parameters.Add(parameterName, customerNumbers[i]);
        }

        string parameterQuery = string.Join(", ", parameterNames);
        string sql =
            $"""
            SELECT 
                A.*,
                U.DISPLAY_NAME AS BearbeiterName
            FROM CUSTOMER_TO_ADDRESS CTA
            INNER JOIN ADDRESSES A ON (A.ADDRESS_ID = CTA.ADDRESS_ID)
            LEFT JOIN USERS U ON (U.USER_ID = A.LAST_MODIFIED_BY)
            WHERE CUSTOMER_NUMBER IN ({parameterQuery})
            """;


        var results = await dbController.SelectDataAsync<Address>(sql, parameters, cancellationToken);

        if (results.Count > 0)
        {
            var countryIds = results.Select(x => x.CountryId).Distinct().ToArray();

            var countries = await _countryService.GetAsync(countryIds, dbController, cancellationToken);

            foreach (var item in results)
            {
                item.Country = countries.FirstOrDefault(x => x.CountryId == item.CountryId);
            }
        }

        return results;
    }

    public Task<List<Address>> GetAsync(int?[] identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task AssignCustomerAsync(Customer input, IDbController dbController, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO CUSTOMER_TO_ADDRESS
            (
                ADDRESS_ID,
                CUSTOMER_NUMBER
            )
            VALUES
            (
                @ADDRESS_ID,
                @CUSTOMER_NUMBER
            )
            """;

        foreach (var item in input.Addresses)
        {
            await dbController.QueryAsync(sql, new
            {
                ADDRESS_ID = item.AddressId,
                CUSTOMER_NUMBER = input.CustomerNumber
            }, cancellationToken);
        }
    }

    public async Task CleanCustomerAssignmentAsync(Customer input, IDbController dbController, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string sql = "DELETE FROM CUSTOMER_TO_ADDRESS WHERE CUSTOMER_NUMBER = @CUSTOMER_NUMBER";

        await dbController.QueryAsync(sql, new
        {
            CUSTOMER_NUMBER = input.CustomerNumber,
        }, cancellationToken);
    }
}
