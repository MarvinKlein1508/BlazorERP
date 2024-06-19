using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Currency : IDbModel<string?>
{
    public string Code { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal ExchangeRate { get; set; }
    public bool MustRound { get; set; }
    public int DecimalPlaces { get; set; }
    public DateTime? RateDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? GetIdentifier() => string.IsNullOrWhiteSpace(Code) ? null : Code;

    
    public string CreatedByName { get; set; } = string.Empty;
    public string LastModifiedName { get; set; } = string.Empty;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "CODE", Code },
            { "RATE", Rate },
            { "EXCHANGE_RATE", ExchangeRate },
            { "MUST_ROUND", MustRound },
            { "DECIMAL_PLACES", DecimalPlaces },
            { "RATE_DATE", RateDate },
            { "CREATED_AT", CreatedAt },
            { "CREATED_BY", CreatedBy },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }
}
