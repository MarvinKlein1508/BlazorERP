using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Nummernkreis : IDbModelWithName<int?>
{
    public int NummernkreisId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int KundeVon { get; set; }
    public int KundeBis { get; set; }
    public int? LetzterBearbeiter { get; set; }
    public DateTime? ZuletztGeaendert { get; set; }
    public int? GetIdentifier() => NummernkreisId > 0 ? NummernkreisId : null;

    public string GetName() => Name;

    public string BearbeiterName { get; set; } = string.Empty;
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "NUMMERNKREIS_ID", NummernkreisId },
            { "NAME", Name },
            { "KUNDE_VON", KundeVon },
            { "KUNDE_BIS", KundeBis },
            { "LETZTER_BEARBEITER", LetzterBearbeiter },
            { "ZULETZT_GEAENDERT", ZuletztGeaendert }
        };
    }
}
