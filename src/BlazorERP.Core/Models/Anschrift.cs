using BlazorERP.Core.Enums;
using BlazorERP.Core.Interfaces;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;

namespace BlazorERP.Core.Models;

public class Anschrift : IDbModel<int?>
{
    public int AnschriftId { get; set; }
    public string? Kundennummer { get; set; }
    public int? Lieferantennummer { get; set; }
    public string Firma { get; set; } = string.Empty;
    public string Name1 { get; set; } = string.Empty;
    public string Name2 { get; set; } = string.Empty;
    public string Strasse { get; set; } = string.Empty;
    public int? LandId { get; set; }
    public int? SprachId { get; set; }
    public string Postleistzahl { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
    public string Telefonnummer { get; set; } = string.Empty;
    public string Mobilnummer { get; set; } = string.Empty;
    public string Faxnummer { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? AnsprechpartnerId { get; set; }
    public string UmsatzsteuerIdentifikationsnummer { get; set; } = string.Empty;
    public string Notiz { get; set; } = string.Empty;
    public int? LetzterBearbeiter { get; set; }
    public DateTime? ZuletztGeaendert { get; set; }

    public int? GetIdentifier() => AnschriftId > 0 ? AnschriftId : null;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "ANSCHRIFT_ID", AnschriftId },
            { "KUNDENNUMMER", Kundennummer },
            { "LIEFERANTENNUMMER", Lieferantennummer },
            { "FIRMA", Firma },
            { "NAME1", Name1 },
            { "NAME2", Name2 },
            { "STRASSE", Strasse },
            { "LAND_ID", LandId },
            { "SPRACH_ID", SprachId },
            { "POSTLEISTZAHL", Postleistzahl },
            { "ORT", Ort },
            { "TELEFONNUMMER", Telefonnummer },
            { "MOBILNUMMER", Mobilnummer },
            { "FAXNUMMER", Faxnummer },
            { "EMAIL", Email },
            { "ANSPRECHPARTNER_ID", AnsprechpartnerId },
            { "UMSATZSTEUER_IDENTIFIKATIONSNUMMER", UmsatzsteuerIdentifikationsnummer },
            { "NOTIZ", Notiz },
            { "LETZTER_BEARBEITER", LetzterBearbeiter },
            { "ZULETZT_GEAENDERT", ZuletztGeaendert }
        };
    }
}
