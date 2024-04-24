using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Zahlungsbedingung : IDbModelWithName<int?>
{   
    public int ZahlungsbedingungId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Nettotage { get; set; }
    public int Skonto1Tage { get; set; }
    public decimal Skonto1Prozent { get; set; }
    public int Skonto2Tage { get; set; }
    public decimal Skonto2Prozent { get; set; }
    public bool IstVorkasse { get; set; }
    public bool IstBarzahlung { get; set; }
    public bool IstAbbuchung { get; set; }
    public bool IstRechnung { get; set; }
    public bool IstAktiv { get; set; }

    public bool VerfuegbarKunde { get; set; }
    public bool VerfuegbarLieferant { get; set; }

    public int? LetzterBearbeiter { get; set; }
    public DateTime? ZuletztGeaendert { get; set; }


    public int? GetIdentifier() => ZahlungsbedingungId > 0 ? ZahlungsbedingungId : null;

    public string GetName() => Name;

    public List<Translation> Übersetzungen { get; set; } = [];


    public string BearbeiterName { get; set; } = string.Empty;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "ZAHLUNGSBEDINGUNG_ID", ZahlungsbedingungId },
            { "NAME", Name },
            { "NETTOTAGE", Nettotage },
            { "SKONTO1_TAGE", Skonto1Tage },
            { "SKONTO1_PROZENT", Skonto1Prozent },
            { "SKONTO2_TAGE", Skonto2Tage },
            { "SKONTO2_PROZENT", Skonto2Prozent },
            { "IST_VORKASSE", IstVorkasse },
            { "IST_BARZAHLUNG", IstBarzahlung },
            { "IST_ABBUCHUNG", IstAbbuchung },
            { "IST_RECHNUNG", IstRechnung },
            { "IST_AKTIV", IstAktiv },
            { "VERFUEGBAR_KUNDE", VerfuegbarKunde },
            { "VERFUEGBAR_LIEFERANT", VerfuegbarLieferant },
            { "LETZTER_BEARBEITER", LetzterBearbeiter },
            { "ZULETZT_GEAENDERT", ZuletztGeaendert },
        };
    }
}
