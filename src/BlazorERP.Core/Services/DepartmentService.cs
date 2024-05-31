using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class DepartmentService : IModelService<Department, int?, DepartmentFilter>, ITranslationCode
{
    private readonly TranslationService _translationService;

    public DepartmentService(TranslationService translationService)
    {
        _translationService = translationService;
    }
    public static string GetTranslationCode() => "DEPARTMENT";

    public async Task CreateAsync(Department input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO DEPARTMENTS
            (
                ACTIVE_DIRECTORY_GROUP_CN,
                LAST_MODIFIED_BY,
                LAST_MODIFIED
            )
            VALUES
            (
                @ACTIVE_DIRECTORY_GROUP_CN,
                @LAST_MODIFIED_BY,
                @LAST_MODIFIED
            ) RETURNING DEPARTMENT_ID;
            """;

        input.DepartmentId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

        foreach (var item in input.Translations)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.DepartmentId;

            await _translationService.CreateAsync(item, dbController, cancellationToken);
        }
    }

    public Task DeleteAsync(Department input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM DEPARTMENTS WHERE DEPARTMENT_ID = @DEPARTMENT_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);  
    }

    public async Task<Department?> GetAsync(int? departmentId, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (departmentId is null)
        {
            return null;
        }

        string sql =
            """
            SELECT 
                D.*,
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName
            FROM DEPARTMENTS D
            LEFT JOIN USERS UC ON (UC.USER_ID = D.CREATED_BY)
            LEFT JOIN USERS UL ON (UL.USER_ID = D.LAST_MODIFIED_BY)
            WHERE 
                DEPARTMENT_ID = @DEPARTMENT_ID
            """;

        var result = await dbController.GetFirstAsync<Department>(sql, new
        {
            DEPARTMENT_ID = departmentId
        }, cancellationToken);

        if (result is not null)
        {
            result.Translations = await _translationService.GetAsync(GetTranslationCode(), result.DepartmentId, dbController, cancellationToken);
        }

        return result;
    }

    public async Task<List<Department>> GetAsync(DepartmentFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                D.*,
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName
            FROM DEPARTMENTS D
            LEFT JOIN USERS UC ON (UC.USER_ID = D.CREATED_BY)
            LEFT JOIN USERS UL ON (UL.USER_ID = D.LAST_MODIFIED_BY)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY DEPARTMENT_ID DESC
        """;

        var results = await dbController.SelectDataAsync<Department>(sql, filter.GetParameters(), cancellationToken);
        
        if (results.Count > 0)
        {
            var landIds = results.Select(x => x.DepartmentId).ToArray();
            var translations = await _translationService.GetAsync(GetTranslationCode(), landIds, dbController, cancellationToken);
            foreach (var item in results)
            {
                item.Translations = translations.Where(x => x.ParentId == item.DepartmentId).ToList();
            }
        }

        return results;
    }

    public Task<List<Department>> GetAsync(int?[] identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public string GetFilterWhere(DepartmentFilter filter)
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

    public Task<int> GetTotalAsync(DepartmentFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM DEPARTMENTS
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public async Task UpdateAsync(Department input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            UPDATE DEPARTMENTS SET 
                ACTIVE_DIRECTORY_GROUP_CN = @ACTIVE_DIRECTORY_GROUP_CN,
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY,
                LAST_MODIFIED = @LAST_MODIFIED
            WHERE
                DEPARTMENT_ID = @DEPARTMENT_ID
            """;

        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);

        await _translationService.ClearAsync(GetTranslationCode(), input.DepartmentId, dbController, cancellationToken);
        foreach (var item in input.Translations)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.DepartmentId;

            await _translationService.CreateAsync(item, dbController, cancellationToken);
        }
    }
}
