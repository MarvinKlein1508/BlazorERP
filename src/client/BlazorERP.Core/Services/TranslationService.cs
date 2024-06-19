using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;

namespace BlazorERP.Core.Services;

public class TranslationService : IModelService<Translation>
{
    public Task CreateAsync(Translation input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            INSERT INTO TRANSLATIONS
            (
                CODE,
                LANGUAGE_ID,
                PARENT_ID,
                VALUE_TEXT,
                VALUE_BLOB
            )
            VALUES
            (
                @CODE,
                @LANGUAGE_ID,
                @PARENT_ID,
                @VALUE_TEXT,
                @VALUE_BLOB
            )
            """;

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);  
    }
    public Task UpdateAsync(Translation input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            UPDATE TRANSLATIONS SET
                VALUE_TEXT = @VALUE_TEXT,
                VALUE_BLOB = @VALUE_BLOB
            WHERE
                    CODE = @CODE
                AND LANGUAGE_ID = @LANGUAGE_ID
                AND PARENT_ID = @PARENT_ID
            """;

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
    public Task DeleteAsync(Translation input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM TRANSLATIONS WHERE CODE = @CODE AND LANGUAGE_ID = @LANGUAGE_ID AND PARENT_ID = @PARENT_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }



    public Task<List<Translation>> GetAsync(string code, int parentId, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            SELECT 
                * 
            FROM TRANSLATIONS 
            WHERE 
                    CODE = @CODE
                AND PARENT_ID = @PARENT_ID
            """;

        return dbController.SelectDataAsync<Translation>(sql, new
        {
            CODE = code,
            PARENT_ID = parentId
        }, cancellationToken);
    }

    public static Task<List<Translation>> GetAsync(string code, IDbController dbController, CancellationToken cancellationToken = default)
    {
        
        string sql =
            $"""
            SELECT 
                * 
            FROM TRANSLATIONS 
            WHERE 
                CODE = @CODE
            """;

        return dbController.SelectDataAsync<Translation>(sql, new
        {
            CODE = code,
        }, cancellationToken);
    }

    public Task<List<Translation>> GetAsync(string code, int[] parentIds, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if(parentIds.Length == 0)
        {
            return Task.FromResult<List<Translation>>([]);
        }
        string sql =
            $"""
            SELECT 
                * 
            FROM TRANSLATIONS 
            WHERE 
                    CODE = @CODE
                AND PARENT_ID IN ({string.Join(",", parentIds)})
            """;

        return dbController.SelectDataAsync<Translation>(sql, new
        {
            CODE = code,
        }, cancellationToken);
    }
    public Task<Translation?> GetAsync(string code, int languageId, int parentId, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            SELECT 
                * 
            FROM TRANSLATIONS 
            WHERE 
                    CODE = @CODE
                AND LANGUAGE_ID = @LANGUAGE_ID
                AND PARENT_ID = @PARENT_ID
            """;

        return dbController.GetFirstAsync<Translation>(sql, new
        {
            CODE = code,
            LANGUAGE_ID = languageId,
            PARENT_ID = parentId
        }, cancellationToken);
    }

    public Task ClearAsync(string code, int parentId, IDbController dbController, CancellationToken cancellationToken)
    {
        string sql = $"DELETE FROM TRANSLATIONS WHERE CODE = @CODE AND PARENT_ID = @PARENT_ID";

        return dbController.QueryAsync(sql, new
        {
            CODE = code,
            PARENT_ID = parentId,
        }, cancellationToken);
    }
}
