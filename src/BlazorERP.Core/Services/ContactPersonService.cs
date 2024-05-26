using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class ContactPersonService : IModelService<ContactPerson, int?, ContactPersonFilter>
{
    private readonly LanguageService _languageService;

    public ContactPersonService(LanguageService languageService)
    {
        _languageService = languageService;
    }
    public Task CreateAsync(ContactPerson input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
                U.DISPLAY_NAME AS BEARBEITER_NAME
        FROM CONTACT_PERSONS CP
        LEFT JOIN USERS U ON (U.USER_ID = CP.LAST_MODIFIED_BY)
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

    public Task UpdateAsync(ContactPerson input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
