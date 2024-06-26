﻿using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class CountryService : IModelService<Country, int?, CountryFilter>, ITranslationCode
{
    private readonly TranslationService _translationService;

    public CountryService(TranslationService translationService)
    {
        _translationService = translationService;
    }

    public async Task CreateAsync(Country input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO COUNTRIES
            (
                ISO2,
                ISO3,
                DIALING_CODE,
                IS_EU,
                IS_ACTIVE,
                CREATED_AT,
                CREATED_BY,
                LAST_MODIFIED_BY,
                LAST_MODIFIED
            )
            VALUES
            (
                @ISO2,
                @ISO3,
                @DIALING_CODE,
                @IS_EU,
                @IS_ACTIVE,
                @CREATED_AT,
                @CREATED_BY,
                @LAST_MODIFIED_BY,
                @LAST_MODIFIED
            ) RETURNING COUNTRY_ID;
            """;



        input.CountryId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

        foreach (var item in input.Translations)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.CountryId;

            await _translationService.CreateAsync(item, dbController, cancellationToken);
        }

    }

    public Task DeleteAsync(Country input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM COUNTRIES WHERE COUNTRY_ID = @COUNTRY_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public static async Task<List<Country>> GetAsync(IDbController dbController)
    {
        string sql = "SELECT * FROM COUNTRIES";

        var results = await dbController.SelectDataAsync<Country>(sql);


        var translations = await TranslationService.GetAsync(GetTranslationCode(), dbController);

        foreach (var item in results)
        {
            item.Translations = translations.Where(x => x.ParentId == item.CountryId).ToList();
        }

        return results;
    }
    public async Task<Country?> GetAsync(int? countryId, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (countryId is null)
        {
            return null;
        }

        string sql =
            """
            SELECT 
                C.*,
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName
            FROM COUNTRIES C 
            LEFT JOIN USERS UC ON (UC.USER_ID = C.CREATED_BY)
            LEFT JOIN USERS UL ON (UL.USER_ID = C.LAST_MODIFIED_BY)
            WHERE COUNTRY_ID = @COUNTRY_ID
            """;

        var result = await dbController.GetFirstAsync<Country>(sql, new
        {
            COUNTRY_ID = countryId
        }, cancellationToken);

        if (result is not null)
        {
            result.Translations = await _translationService.GetAsync(GetTranslationCode(), result.CountryId, dbController, cancellationToken);
        }

        return result;
    }
    public async Task<List<Country>> GetAsync(int?[] identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (identifiers.Length is 0)
        {
            return [];
        }

        var parameters = new Dictionary<string, object?>();

        List<string> parameterNames = [];

        for (int i = 0; i < identifiers.Length; i++)
        {
            string parameterName = $"@COUNTRY_ID{i}";
            parameterNames.Add(parameterName);
            parameters.Add(parameterName, identifiers[i]);
        }

        string parameterQuery = string.Join(", ", parameterNames);

        string sql =
        $"""
        SELECT 
            C.*,
            UC.DISPLAY_NAME AS CreatedByName,
            UL.DISPLAY_NAME AS LastModifiedName
        FROM COUNTRIES C 
        LEFT JOIN USERS UC ON (UC.USER_ID = C.CREATED_BY)
        LEFT JOIN USERS UL ON (UL.USER_ID = C.LAST_MODIFIED_BY)
        WHERE 1 = 1 AND COUNTRY_ID IN 
        (
            {parameterQuery}
        )
        ORDER BY COUNTRY_ID
        """;

        var results = await dbController.SelectDataAsync<Country>(sql, parameters, cancellationToken);
        if (results.Count > 0)
        {
            var countryIds = results.Select(x => x.CountryId).Distinct().ToArray();
            var translations = await _translationService.GetAsync(GetTranslationCode(), countryIds, dbController, cancellationToken);
            foreach (var item in results)
            {
                item.Translations = translations.Where(x => x.ParentId == item.CountryId).ToList();
            }
        }

        return results;
    }
    public async Task<List<Country>> GetAsync(CountryFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                C.*,
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName
            FROM COUNTRIES C 
            LEFT JOIN USERS UC ON (UC.USER_ID = C.CREATED_BY)
            LEFT JOIN USERS UL ON (UL.USER_ID = C.LAST_MODIFIED_BY)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY COUNTRY_ID
        """;

        var results = await dbController.SelectDataAsync<Country>(sql, filter.GetParameters(), cancellationToken);
        if (results.Count > 0)
        {
            var countryIds = results.Select(x => x.CountryId).ToArray();
            var translations = await _translationService.GetAsync(GetTranslationCode(), countryIds, dbController, cancellationToken);
            foreach (var item in results)
            {
                item.Translations = translations.Where(x => x.ParentId == item.CountryId).ToList();
            }
        }

        return results;
    }

    public string GetFilterWhere(CountryFilter filter)
    {
        StringBuilder sb = new();
       

        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@$" AND 

        C.COUNTRY_ID IN 
        (
            SELECT 
                PARENT_ID
            FROM TRANSLATIONS T
            WHERE   
                T.CODE = '{GetTranslationCode()}'
                AND UPPER(T.VALUE_TEXT) LIKE @SEARCH_PHRASE
        )");
        }

        


        string sql = sb.ToString();
        return sql;
    }

    public Task<int> GetTotalAsync(CountryFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM COUNTRIES C
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public static string GetTranslationCode() => "COUNTRY";

    public async Task UpdateAsync(Country input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE COUNTRIES SET 
                ISO2 = @ISO2,
                ISO3 = @ISO3,
                DIALING_CODE = @DIALING_CODE,
                IS_EU = @IS_EU,
                IS_ACTIVE = @IS_ACTIVE,
                CREATED_AT = @CREATED_AT,
                CREATED_BY = @CREATED_BY,
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY,
                LAST_MODIFIED = @LAST_MODIFIED
            WHERE
                COUNTRY_ID = @COUNTRY_ID
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);


        await _translationService.ClearAsync(GetTranslationCode(), input.CountryId, dbController, cancellationToken);
        foreach (var item in input.Translations)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.CountryId;

            await _translationService.CreateAsync(item, dbController, cancellationToken);
        }
    }

    
}
