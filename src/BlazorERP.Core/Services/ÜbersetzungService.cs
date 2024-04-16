using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;

namespace BlazorERP.Core.Services;

public class ÜbersetzungService : IModelService<Übersetzung>
{
    public Task CreateAsync(Übersetzung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            INSERT INTO UEBERSETZUNGEN
            (
                CODE,
                SPRACH_ID,
                PARENT_ID,
                VALUE_TEXT,
                VALUE_BLOB
            )
            VALUES
            (
                @CODE,
                @SPRACH_ID,
                @PARENT_ID,
                @VALUE_TEXT,
                @VALUE_BLOB
            )
            """;

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);  
    }
    public Task UpdateAsync(Übersetzung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            UPDATE UEBERSETZUNGEN SET
                VALUE_TEXT = @VALUE_TEXT,
                VALUE_BLOB = @VALUE_BLOB
            WHERE
                    CODE = @CODE
                AND SPRACH_ID = @SPRACH_ID
                AND PARENT_ID = @PARENT_ID
            """;

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
    public Task DeleteAsync(Übersetzung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM UEBERSETZUNGEN WHERE CODE = @CODE AND SPRACH_ID = @SPRACH_ID AND PARENT_ID = @PARENT_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }



    public Task<List<Übersetzung>> GetAsync(string code, int parentId, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            SELECT 
                * 
            FROM UEBERSETZUNGEN 
            WHERE 
                    CODE = @CODE
                AND PARENT_ID = @PARENT_ID
            """;

        return dbController.SelectDataAsync<Übersetzung>(sql, new
        {
            CODE = code,
            PARENT_ID = parentId
        }, cancellationToken);
    }

    public Task<List<Übersetzung>> GetAsync(string code, int[] parentIds, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if(parentIds.Length == 0)
        {
            return Task.FromResult<List<Übersetzung>>([]);
        }
        string sql =
            $"""
            SELECT 
                * 
            FROM UEBERSETZUNGEN 
            WHERE 
                    CODE = @CODE
                AND PARENT_ID IN ({string.Join(",", parentIds)})
            """;

        return dbController.SelectDataAsync<Übersetzung>(sql, new
        {
            CODE = code,
        }, cancellationToken);
    }
    public Task<Übersetzung?> GetAsync(string code, int sprachId, int parentId, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            SELECT 
                * 
            FROM UEBERSETZUNGEN 
            WHERE 
                    CODE = @CODE
                AND SPRACH_ID = @SPRACH_ID
                AND PARENT_ID = @PARENT_ID
            """;

        return dbController.GetFirstAsync<Übersetzung>(sql, new
        {
            CODE = code,
            SPRACH_ID = sprachId,
            PARENT_ID = parentId
        }, cancellationToken);
    }

    public Task ClearAsync(string code, int parentId, IDbController dbController, CancellationToken cancellationToken)
    {
        string sql = $"DELETE FROM UEBERSETZUNGEN WHERE CODE = @CODE AND PARENT_ID = @PARENT_ID";

        return dbController.QueryAsync(sql, new
        {
            CODE = code,
            PARENT_ID = parentId,
        }, cancellationToken);
    }
}
