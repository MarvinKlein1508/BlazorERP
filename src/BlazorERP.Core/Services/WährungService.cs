using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class WährungService : IModelService<Währung, string?, WährungFilter>
{
    public Task CreateAsync(Währung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO WAEHRUNG
            (
                CODE,
                KURS,
                WECHSELKURS,
                MUSS_RUNDEN,
                NACHKOMMASTELLEN,
                KURS_VOM,
                LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT
            )
            VALUES
            (
                @CODE,
                @KURS,
                @WECHSELKURS,
                @MUSS_RUNDEN,
                @NACHKOMMASTELLEN,
                @KURS_VOM,
                @LETZTER_BEARBEITER,
                @ZULETZT_GEAENDERT
            );
            """;

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public Task DeleteAsync(Währung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM WAEHRUNG WHERE CODE = @CODE";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
    public static Task<List<Währung>> GetAsync(IDbController dbController)
    {
        string sql = "SELECT * FROM WAEHRUNG";

        return dbController.SelectDataAsync<Währung>(sql);

    }
    public Task<Währung?> GetAsync(string? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return Task.FromResult<Währung?>(null);
        }

        string sql =
            """
            SELECT 
                W.*,
                U.ANZEIGENAME AS BEARBEITER_NAME
            FROM WAEHRUNG W
            LEFT JOIN USERS U ON (u.USER_ID = W.LETZTER_BEARBEITER)
            WHERE CODE = @CODE
            """;

        return dbController.GetFirstAsync<Währung>(sql, new
        {
            CODE = identifier
        }, cancellationToken);
    }

    public Task<List<Währung>> GetAsync(WährungFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                W.*,
                U.ANZEIGENAME AS BEARBEITER_NAME
            FROM WAEHRUNG W
            LEFT JOIN USERS U ON (u.USER_ID = W.LETZTER_BEARBEITER)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY CODE DESC
        """;

        return dbController.SelectDataAsync<Währung>(sql, filter.GetParameters(), cancellationToken);

    }



    public string GetFilterWhere(WährungFilter filter)
    {
        StringBuilder sb = new();

        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@" AND 
(
        UPPER(CODE) LIKE @SEARCH_PHRASE
)");
        }



        string sql = sb.ToString();
        return sql;
    }

    public Task<int> GetTotalAsync(WährungFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM WAEHRUNG
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public Task UpdateAsync(Währung input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE WAEHRUNG SET 
                KURS = @KURS,
                WECHSELKURS = @WECHSELKURS,
                MUSS_RUNDEN = @MUSS_RUNDEN,
                NACHKOMMASTELLEN = @NACHKOMMASTELLEN,
                KURS_VOM = @KURS_VOM,
                LETZTER_BEARBEITER = @LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT = @ZULETZT_GEAENDERT
            WHERE
                CODE = @CODE
            """;


        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
}
