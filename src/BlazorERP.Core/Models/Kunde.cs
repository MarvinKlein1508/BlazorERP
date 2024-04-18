using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Kunde : IDbModelWithName<string?>
{
    public string Kundennummer { get; set; } = string.Empty;
    public string Firma { get; set; } = string.Empty;
    public string Name1 { get; set; } = string.Empty;
    public string Name2 { get; set; } = string.Empty;

    public DateTime? Anlagedatum { get; set; }
    public int? SprachId { get; set; }
    public int? LandId { get; set; }
    public string Strasse { get; set; } = string.Empty;
    public string Postleitzahl { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;

    public string Telefonnummer { get; set; } = string.Empty;
    public string Mobilnummer { get; set; } = string.Empty;
    public string Faxnummer { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Notiz { get; set; } = string.Empty;
    public int? AnredeId { get; set; }
    public int? ZahlungsbedingungId { get; set; }
    public int? LieferbedingungId { get; set; }
    public string UmsatzsteuerIdentifikationsnummer { get; set; } = string.Empty;
    public decimal Kreditlimit { get; set; }
    public string IBAN { get; set; } = string.Empty;
    public string BIC { get; set; } = string.Empty;
    public bool IstGesperrt { get; set; }
    public bool NeutralerVersand { get; set; }
    public string? Waehrungscode { get; set; }
    public int? LetzterBearbeiter { get; set; }
    public DateTime? ZuletztGeaendert { get; set; }

    public string? GetIdentifier() => string.IsNullOrWhiteSpace(Kundennummer) ? null : Kundennummer;

    public string GetName()
    {
        if(!string.IsNullOrWhiteSpace(Firma))
        {
            return Firma;
        }

        if(!string.IsNullOrWhiteSpace(Name1))
        {
            return Name1;
        }

        return string.Empty;
    }

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "KUNDENNUMMER", Kundennummer },
            { "FIRMA", Firma },
            { "NAME1", Name1 },
            { "NAME2", Name2 },
            { "ANLAGEDATUM", Anlagedatum },
            { "SPRACH_ID", SprachId },
            { "LAND_ID", LandId },
            { "STRASSE", Strasse },
            { "POSTLEITZAHL", Postleitzahl },
            { "ORT", Ort },
            { "TELEFONNUMMER", Telefonnummer },
            { "MOBILNUMMER", Mobilnummer },
            { "FAXNUMMER", Faxnummer },
            { "EMAIL", Email },
            { "WEBSITE", Website },
            { "NOTIZ", Notiz },
            { "ANREDE_ID", AnredeId },
            { "ZAHLUNGSBEDINGUNG_ID", ZahlungsbedingungId },
            { "LIEFERBEDINGUNG_ID", LieferbedingungId },
            { "UMSATZSTEUER_IDENTIFIKATIONSNUMMER", UmsatzsteuerIdentifikationsnummer },
            { "KREDITLIMIT", Kreditlimit },
            { "IBAN", IBAN },
            { "BIC", BIC },
            { "IST_GESCHLOSSEN", IstGesperrt },
            { "NEUTRALER_VERSAND", NeutralerVersand },
            { "WAEHRUNGSCODE", Waehrungscode },
            { "LETZTER_BEARBEITER", LetzterBearbeiter },
            { "ZULETZT_GEAENDERT", ZuletztGeaendert },
        };
    }
}
