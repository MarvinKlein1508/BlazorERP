using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace BlazorERP.Core.Services;

public class ContactPersonService : IModelService<ContactPerson, int?, ContactPersonFilter>
{
    private readonly LanguageService _languageService;

    public ContactPersonService(LanguageService languageService)
    {
        _languageService = languageService;
    }
    public async Task CreateAsync(ContactPerson input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string sql =
            """
            INSERT INTO CONTACT_PERSONS
            (
                COMPANY,
                SALUTATION_ID,
                FIRSTNAME,
                LASTNAME,
                DEPARTMENT,
                LANGUAGE_ID,
                PHONE_NUMBER,
                MOBILE_NUMBER,
                FAX_NUMBER,
                EMAIL,
                NOTE,
                CREATED_AT,
                CREATED_BY,
                LAST_MODIFIED_BY,
                LAST_MODIFIED
            )
            VALUES
            (
                @COMPANY,
                @SALUTATION_ID,
                @FIRSTNAME,
                @LASTNAME,
                @DEPARTMENT,
                @LANGUAGE_ID,
                @PHONE_NUMBER,
                @MOBILE_NUMBER,
                @FAX_NUMBER,
                @EMAIL,
                @NOTE,
                @CREATED_AT,
                @CREATED_BY,
                @LAST_MODIFIED_BY,
                @LAST_MODIFIED
            ) RETURNING CONTACT_PERSON_ID;
            """;

        input.ContactPersonId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
    }

    public Task DeleteAsync(ContactPerson input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ContactPerson?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ContactPerson>> GetAsync(int?[] identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ContactPerson>> GetAsync(ContactPersonFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                CP.*,
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName
        FROM CONTACT_PERSONS CP
        LEFT JOIN USERS UC ON (UC.USER_ID = CP.CREATED_BY)
        LEFT JOIN USERS UL ON (UL.USER_ID = CP.LAST_MODIFIED_BY)
        WHERE 1 = 1
            {GetFilterWhere(filter)}
        ORDER BY CONTACT_PERSON_ID DESC
        """;

        var results = await dbController.SelectDataAsync<ContactPerson>(sql, filter.GetParameters(), cancellationToken);

        if (results.Count > 0)
        {
            var languageIds = results.Select(x => x.LanguageId).ToArray();

            var languages = await _languageService.GetAsync(languageIds, dbController, cancellationToken);

            foreach (var item in results)
            {
                item.Language = languages.FirstOrDefault(x => x.LanguageId == item.LanguageId);
            }
        }

        return results;
    }

    public string GetFilterWhere(ContactPersonFilter filter)
    {
        StringBuilder sb = new();

        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@" AND 
(
        UPPER(COMPANY) LIKE @SEARCH_PHRASE
    OR  UPPER(FIRSTNAME) LIKE @SEARCH_PHRASE
    OR  UPPER(LASTNAME) LIKE @SEARCH_PHRASE
)");
        }

        if (filter.Blocked.Count > 0)
        {
            sb.AppendLine($"AND CONTACT_PERSON_ID NOT IN ({filter.GetBlockedQuery()})");
        }


        string sql = sb.ToString();
        return sql;
    }

    public async Task<int> GetTotalAsync(ContactPersonFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM CONTACT_PERSONS
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        var result = await dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);

        return result;
    }

    public async Task UpdateAsync(ContactPerson input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string sql =
            """
            UPDATE CONTACT_PERSONS SET
                COMPANY = @COMPANY,
                SALUTATION_ID = @SALUTATION_ID,
                FIRSTNAME = @FIRSTNAME,
                LASTNAME = @LASTNAME,
                DEPARTMENT = @DEPARTMENT,
                LANGUAGE_ID = @LANGUAGE_ID,
                PHONE_NUMBER = @PHONE_NUMBER,
                MOBILE_NUMBER = @MOBILE_NUMBER,
                FAX_NUMBER = @FAX_NUMBER,
                EMAIL = @EMAIL,
                NOTE = @NOTE,
                CREATED_AT = @CREATED_AT,
                CREATED_BY = @CREATED_BY,
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY,
                LAST_MODIFIED = @LAST_MODIFIED
            WHERE
                CONTACT_PERSON_ID = @CONTACT_PERSON_ID
            """;

        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
}
