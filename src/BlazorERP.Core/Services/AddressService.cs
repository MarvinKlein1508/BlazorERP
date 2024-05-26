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
            LEFT JOIN COUNTRIES C ON (C.COUNTRY_ID = A.COUNTRY_ID)
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
                C.*,
                TC.*,
                U.DISPLAY_NAME AS BEARBEITER_NAME
        FROM ADDRESSES A
        LEFT JOIN USERS U ON (U.USER_ID = A.LAST_MODIFIED_BY)
        LEFT JOIN COUNTRIES C ON (C.COUNTRY_ID = A.COUNTRY_ID)
        LEFT JOIN TRANSLATIONS TC ON (TC.CODE = 'COUNTRY' AND TC.PARENT_ID = C.COUNTRY_ID)
        WHERE 1 = 1
            {GetFilterWhere(filter)}
        ORDER BY ADDRESS_ID DESC
        """;
        var addressDict = new Dictionary<int, Address>();

        var results =
        (
            await dbController.SelectDataAsync<Address, Country, Translation, Address>(sql, (address, country, translation) =>
            {
                if (!addressDict.TryGetValue(address.AddressId, out var currentAddress))
                {
                    currentAddress = address;
                    currentAddress.Country = country;
                    currentAddress.Country.Translations = new List<Translation>();
                    addressDict.Add(address.AddressId, currentAddress);
                }

                if (translation != null && !string.IsNullOrEmpty(translation.Code))
                {
                    currentAddress.Country.Translations.Add(translation);
                }

                return currentAddress;
            }, "country_id, code", filter.GetParameters(), cancellationToken)
        )
        .Distinct()
        .ToList();

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
                C.*,
                TC.*,
                U.DISPLAY_NAME AS BearbeiterName
            FROM CUSTOMER_TO_ADDRESS CTA
            INNER JOIN ADDRESSES A ON (A.ADDRESS_ID = CTA.ADDRESS_ID)
            LEFT JOIN USERS U ON (U.USER_ID = A.LAST_MODIFIED_BY)
            LEFT JOIN COUNTRIES C ON (C.COUNTRY_ID = A.COUNTRY_ID)
            LEFT JOIN TRANSLATIONS TC ON (TC.CODE = 'COUNTRY' AND TC.PARENT_ID = C.COUNTRY_ID)
            WHERE CUSTOMER_NUMBER IN ({parameterQuery})
            """;

        var addressDict = new Dictionary<int, Address>();

        var results =
        (
            await dbController.SelectDataAsync<Address, Country, Translation, Address>(sql, (address, country, translation) =>
            {
                if (!addressDict.TryGetValue(address.AddressId, out var currentAddress))
                {
                    currentAddress = address;
                    currentAddress.Country = country;
                    currentAddress.Country.Translations = new List<Translation>();
                    addressDict.Add(address.AddressId, currentAddress);
                }

                if (translation != null && !string.IsNullOrEmpty(translation.Code))
                {
                    currentAddress.Country!.Translations.Add(translation);
                }

                return currentAddress;
            }, "country_id, code", parameters, cancellationToken)
        )
        .Distinct()
        .ToList();

        return results;
    }
}
