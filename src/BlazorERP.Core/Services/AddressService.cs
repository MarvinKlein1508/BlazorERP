using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class AddressService : IModelService<Address, int?, AddressFilter>
{
    public async Task CreateAsync(Address input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO ANSCHRIFTEN
            (              
                KUNDENNUMMER,
                LIEFERANTENNUMMER,
                FIRMA,
                NAME1,
                NAME2,
                STRASSE,
                LAND_ID,
                SPRACH_ID,
                POSTLEISTZAHL,
                ORT,
                TELEFONNUMMER,
                MOBILNUMMER,
                FAXNUMMER,
                EMAIL,
                ANSPRECHPARTNER_ID,
                UMSATZSTEUER_IDENTIFIKATIONSNUMMER,
                NOTIZ,
                LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT
            )
            VALUES
            (
                @KUNDENNUMMER,
                @LIEFERANTENNUMMER,
                @FIRMA,
                @NAME1,
                @NAME2,
                @STRASSE,
                @LAND_ID,
                @SPRACH_ID,
                @POSTLEISTZAHL,
                @ORT,
                @TELEFONNUMMER,
                @MOBILNUMMER,
                @FAXNUMMER,
                @EMAIL,
                @ANSPRECHPARTNER_ID,
                @UMSATZSTEUER_IDENTIFIKATIONSNUMMER,
                @NOTIZ,
                @LETZTER_BEARBEITER,
                @ZULETZT_GEAENDERT
            ) RETURNING ANSCHRIFT_ID;
            """;



        input.AnschriftId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
    }

    public Task DeleteAsync(Address input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM ANSCHRIFTEN WHERE ANSCHRIFT_ID = @ANSCHRIFT_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);  
    }

    public Task<Address?> GetAsync(int? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return Task.FromResult<Address?>(null);
        }

        string sql =
            """
            SELECT 
                A.*,
                U.ANZEIGENAME AS BEARBEITER_NAME
            FROM ANSCHRIFTEN A 
            LEFT JOIN USERS U ON (U.USER_ID = A.LETZTER_BEARBEITER)
            WHERE 
                ANSCHRIFT_ID = @ANSCHRIFT_ID
            """;

        return dbController.GetFirstAsync<Address>(sql, new
        {
            ANSCHRIFT_ID = identifier
        }, cancellationToken);
    }

    public Task<List<Address>> GetAsync(AddressFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                A.*,
                U.ANZEIGENAME AS BEARBEITER_NAME
            FROM ANSCHRIFTEN A
            LEFT JOIN USERS U ON (U.USER_ID = A.LETZTER_BEARBEITER)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY ANSCHRIFT_ID DESC
        """;

        return dbController.SelectDataAsync<Address>(sql, filter.GetParameters(), cancellationToken);
    }

    public string GetFilterWhere(AddressFilter filter)
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

    public Task<int> GetTotalAsync(AddressFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM ANSCHRIFTEN
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public Task UpdateAsync(Address input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            UPDATE ANSCHRIFTEN SET 
                KUNDENNUMMER = @KUNDENNUMMER,
                LIEFERANTENNUMMER = @LIEFERANTENNUMMER,
                FIRMA = @FIRMA,
                NAME1 = @NAME1,
                NAME2 = @NAME2,
                STRASSE = @STRASSE,
                LAND_ID = @LAND_ID,
                SPRACH_ID = @SPRACH_ID,
                POSTLEISTZAHL = @POSTLEISTZAHL,
                ORT = @ORT,
                TELEFONNUMMER = @TELEFONNUMMER,
                MOBILNUMMER = @MOBILNUMMER,
                FAXNUMMER = @FAXNUMMER,
                EMAIL = @EMAIL,
                ANSPRECHPARTNER_ID = @ANSPRECHPARTNER_ID,
                UMSATZSTEUER_IDENTIFIKATIONSNUMMER = @UMSATZSTEUER_IDENTIFIKATIONSNUMMER,
                NOTIZ = @NOTIZ,
                LETZTER_BEARBEITER = @LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT = @ZULETZT_GEAENDERT
            WHERE
                ANSCHRIFT_ID = @ANSCHRIFT_ID
            """;

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
}
