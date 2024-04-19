using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Land : IDbModelWithName<int?>
{
    public int LandId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ISO2 { get; set; } = string.Empty;
    public string ISO3 { get; set; } = string.Empty;
    public string Vorwahl { get; set; } = string.Empty;

    public bool IstEU { get; set; }
    public int? LetzterBearbeiter { get; set; }
    public DateTime? ZuletztGeaendert { get; set; }
    public List<Übersetzung> Übersetzungen { get; set; } = [];
    public int? GetIdentifier() => LandId > 0 ? LandId : null;

    public string GetName() => Name;

    public string BearbeiterName { get; set; } = string.Empty;
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "LAND_ID", LandId },
            { "NAME", Name },
            { "ISO2", ISO2 },
            { "ISO3", ISO3 },
            { "VORWAHL", Vorwahl },
            { "IST_EU", IstEU },
            { "LETZTER_BEARBEITER", LetzterBearbeiter },
            { "ZULETZT_GEAENDERT", ZuletztGeaendert },
        };
    }
}
