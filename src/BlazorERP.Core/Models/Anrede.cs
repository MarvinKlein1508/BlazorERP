using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Anrede : IDbModelWithName<int?>
{   
    public int AnredeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? LetzterBearbeiter { get; set; }
    public DateTime ZuletztGeaendert { get; set; }


    public int? GetIdentifier() => AnredeId > 0 ? AnredeId : null;

    public string GetName() => Name;

    public List<Übersetzung> Übersetzungen { get; set; } = [];

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "ANREDE_ID", AnredeId },
            { "NAME", Name },
            { "LETZTER_BEARBEITER", LetzterBearbeiter },
            { "ZULETZT_GEAENDERT", ZuletztGeaendert },
        };
    }
}
