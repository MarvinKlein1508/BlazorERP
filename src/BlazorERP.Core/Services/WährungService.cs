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
                ZEICHEN,
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
                @ZEICHEN,
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
        string sql = "DELETE FROM WAEHRUNG WHERE ZEICHEN = @ZEICHEN";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public Task<Währung?> GetAsync(string? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return Task.FromResult<Währung?>(null);
        }

        string sql = "SELECT * FROM WAEHRUNG WHERE ZEICHEN = @ZEICHEN";

        return dbController.GetFirstAsync<Währung>(sql, new
        {
            ZEICHEN = identifier
        }, cancellationToken);
    }

    public Task<List<Währung>> GetAsync(WährungFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                * 
            FROM WAEHRUNG 
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY ZEICHEN DESC
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
        UPPER(ZEICHEN) LIKE @SEARCH_PHRASE
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
                ZEICHEN = @ZEICHEN
            """;


        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
}
