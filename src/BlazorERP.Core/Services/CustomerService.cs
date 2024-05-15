using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class CustomerService : IModelService<Customer, string?, CustomerFilter>
{
    public async Task CreateAsync(Customer input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO KUNDEN
            (
                KUNDENNUMMER,
                FIRMA,
                NAME1,
                NAME2,
                ANLAGEDATUM,
                SPRACH_ID,
                LAND_ID,
                STRASSE,
                POSTLEITZAHL,
                ORT,
                TELEFONNUMMER,
                MOBILNUMMER,
                FAXNUMMER,
                EMAIL,
                WEBSITE,
                NOTIZ,
                ANREDE_ID,
                ZAHLUNGSBEDINGUNG_ID,
                LIEFERBEDINGUNG_ID,
                UMSATZSTEUER_IDENTIFIKATIONSNUMMER,
                KREDITLIMIT,
                IBAN,
                BIC,
                IST_GESPERRT,
                NEUTRALER_VERSAND,
                WAEHRUNGSCODE,
                LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT
            )
            VALUES
            (
                @KUNDENNUMMER,
                @FIRMA,
                @NAME1,
                @NAME2,
                @ANLAGEDATUM,
                @SPRACH_ID,
                @LAND_ID,
                @STRASSE,
                @POSTLEITZAHL,
                @ORT,
                @TELEFONNUMMER,
                @MOBILNUMMER,
                @FAXNUMMER,
                @EMAIL,
                @WEBSITE,
                @NOTIZ,
                @ANREDE_ID,
                @ZAHLUNGSBEDINGUNG_ID,
                @LIEFERBEDINGUNG_ID,
                @UMSATZSTEUER_IDENTIFIKATIONSNUMMER,
                @KREDITLIMIT,
                @IBAN,
                @BIC,
                @IST_GESPERRT,
                @NEUTRALER_VERSAND,
                @WAEHRUNGSCODE,
                @LETZTER_BEARBEITER,
                @ZULETZT_GEAENDERT
            ) RETURNING KUNDENNUMMER;
            """;

        input.Kundennummer = (await dbController.GetFirstAsync<string>(sql, input.GetParameters(), cancellationToken))!;
    }

    public Task DeleteAsync(Customer input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM KUNDEN WHERE KUNDENNUMMER = @KUNDENNUMMER";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public Task<Customer?> GetAsync(string? identifier, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return Task.FromResult<Customer?>(null);
        }

        string sql = "SELECT * FROM KUNDEN WHERE KUNDENNUMMER = @KUNDENNUMMER";

        return dbController.GetFirstAsync<Customer>(sql, new
        {
            KUNDENNUMMER = identifier
        }, cancellationToken);
    }

    public Task<List<Customer>> GetAsync(CustomerFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                * 
            FROM KUNDEN 
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY KUNDENNUMMER DESC
        """;

        return dbController.SelectDataAsync<Customer>(sql, filter.GetParameters(), cancellationToken);
    }

 

    public string GetFilterWhere(CustomerFilter filter)
    {
        StringBuilder sb = new();

        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@" AND 
(
        UPPER(FIRMA) LIKE @SEARCH_PHRASE
    OR  UPPER(NAME1) LIKE @SEARCH_PHRASE
    OR  UPPER(NAME2) LIKE @SEARCH_PHRASE
)");
        }



        string sql = sb.ToString();
        return sql;
    }

    public Task<int> GetTotalAsync(CustomerFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM KUNDEN
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public async Task UpdateAsync(Customer input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE KUNDEN SET 
                FIRMA = @FIRMA,
                NAME1 = @NAME1,
                NAME2 = @NAME2,
                ANLAGEDATUM = @ANLAGEDATUM,
                SPRACH_ID = @SPRACH_ID,
                LAND_ID = @LAND_ID,
                STRASSE = @STRASSE,
                POSTLEITZAHL = @POSTLEITZAHL,
                ORT = @ORT,
                TELEFONNUMMER = @TELEFONNUMMER,
                MOBILNUMMER = @MOBILNUMMER,
                FAXNUMMER = @FAXNUMMER,
                EMAIL = @EMAIL,
                WEBSITE = @WEBSITE,
                NOTIZ = @NOTIZ,
                ANREDE_ID = @ANREDE_ID,
                ZAHLUNGSBEDINGUNG_ID = @ZAHLUNGSBEDINGUNG_ID,
                LIEFERBEDINGUNG_ID = @LIEFERBEDINGUNG_ID,
                UMSATZSTEUER_IDENTIFIKATIONSNUMMER = @UMSATZSTEUER_IDENTIFIKATIONSNUMMER,
                KREDITLIMIT = @KREDITLIMIT,
                IBAN = @IBAN,
                BIC = @BIC,
                IST_GESPERRT = @IST_GESPERRT,
                NEUTRALER_VERSAND = @NEUTRALER_VERSAND,
                WAEHRUNGSCODE = @WAEHRUNGSCODE,
                LETZTER_BEARBEITER = @LETZTER_BEARBEITER,
                ZULETZT_GEAENDERT = @ZULETZT_GEAENDERT
            WHERE
                KUNDENNUMMER = @KUNDENNUMMER
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }
}
