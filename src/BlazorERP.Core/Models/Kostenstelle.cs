using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Kostenstelle : IDbModelWithName<int?>
{
    public int Nummer { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? LetzterBearbeiter { get; set; }
    public DateTime? ZuletztGeaendert { get; set; }


    public string BearbeiterName { get; set; } = string.Empty;
    public int? GetIdentifier() => Nummer > 0 ? Nummer : null;

    public string GetName() => Name;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "NUMMER", Nummer },
            { "NAME", Name },
            { "LETZTER_BEARBEITER", LetzterBearbeiter },
            { "ZULETZT_GEAENDERT", ZuletztGeaendert },
        };
    }
}
