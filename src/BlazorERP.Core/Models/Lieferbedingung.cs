using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Lieferbedingung : IDbModelWithName<int?>
{   
    public int LieferbedingungId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool VersandMitSpedition { get; set; }
    public bool IstAbholung { get; set; }
    public bool VerfuegbarKunde { get; set; }
    public bool VerfuegbarLieferant { get; set; }
    public bool IstAktiv { get; set; }

    public int? LetzterBearbeiter { get; set; }
    public DateTime? ZuletztGeaendert { get; set; }


    public int? GetIdentifier() => LieferbedingungId > 0 ? LieferbedingungId : null;

    public string GetName() => Name;

    public List<Translation> Übersetzungen { get; set; } = [];


    public string BearbeiterName { get; set; } = string.Empty;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "LIEFERBEDINGUNG_ID", LieferbedingungId },
            { "NAME", Name },
            { "VERSAND_MIT_SPEDITION", VersandMitSpedition },
            { "IST_ABHOLUNG", IstAbholung },
            { "VERFUEGBAR_KUNDE", VerfuegbarKunde },
            { "VERFUEGBAR_LIEFERANT", VerfuegbarLieferant },
            { "IST_AKTIV", IstAktiv },
            { "LETZTER_BEARBEITER", LetzterBearbeiter },
            { "ZULETZT_GEAENDERT", ZuletztGeaendert },
        };
    }
}
