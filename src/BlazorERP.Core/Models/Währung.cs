using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Währung : IDbModelWithName<string?>
{
    public string Code { get; set; } = string.Empty;
    public decimal Kurs { get; set; }
    public decimal Wechselkurs { get; set; }
    public bool MussRunden { get; set; }
    public int Nachkommastellen { get; set; }
    public DateTime? KursVom { get; set; }
    public int? LetzterBearbeiter { get; set; }
    public DateTime? ZuletztGeaendert { get; set; }
    public string? GetIdentifier() => string.IsNullOrWhiteSpace(Code) ? null : Code;
    public string GetName() => Code;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "CODE", Code },
            { "KURS", Code },
            { "WECHSELKURS", Code },
            { "MUSS_RUNDEN", Code },
            { "NACHKOMMASTELLEN", Code },
            { "KURS_VOM", KursVom },
            { "LETZTER_BEARBEITER", LetzterBearbeiter },
            { "ZULETZT_GEAENDERT", ZuletztGeaendert },
        };
    }
}
