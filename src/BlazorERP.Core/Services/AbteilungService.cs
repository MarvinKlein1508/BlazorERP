using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class AbteilungService : IModelService<Abteilung, int?, AbteilungFilter>
{
    public async Task CreateAsync(Abteilung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO ABTEILUNGEN
            (
                NAME,
                ACTIVE_DIRECTORY_GROUP_CN
            )
            VALUES
            (
                @NAME,
                @ACTIVE_DIRECTORY_GROUP_CN
            ) RETURNING ABTEILUNG_ID;
            """;



        input.AbteilungId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
    }

    public Task DeleteAsync(Abteilung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE * FROM ABTEILUNGEN WHERE ABTEILUNG_ID = @ABTEILUNG_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);  
    }

    public Task<Abteilung?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return Task.FromResult<Abteilung?>(null);
        }

        string sql = "SELECT * FROM ABTEILUNGEN WHERE ABTEILUNG_ID = @ABTEILUNG_ID";

        return dbController.GetFirstAsync<Abteilung>(sql, new
        {
            ABTEILUNG_ID = identifier
        }, cancellationToken);
    }

    public Task<List<Abteilung>> GetAsync(AbteilungFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                * 
            FROM ABTEILUNGEN 
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY ABTEILUNG_ID DESC
        """;

        return dbController.SelectDataAsync<Abteilung>(sql, filter.GetParameters(), cancellationToken);
    }

    public string GetFilterWhere(AbteilungFilter filter)
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

    public Task<int> GetTotalAsync(AbteilungFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM ABTEILUNGEN
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public Task UpdateAsync(Abteilung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            UPDATE ABTEILUNGEN SET 
                NAME = @NAME
                ACTIVE_DIRECTORY_GROUP_CN = @ACTIVE_DIRECTORY_GROUP_CN
            WHERE
                ABTEILUNG_ID = @ABTEILUNG_ID
            """;

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
}
