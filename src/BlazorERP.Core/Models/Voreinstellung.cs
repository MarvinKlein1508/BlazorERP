using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Voreinstellung : IDbModelWithName<int?>
{
    public int VoreinstellungId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? KundeLieferbedingungId { get;set;}
    public int? KundeZahlungsbedingungId { get;set;}
    public int? KundeAnredeId { get;set;}
    public string? KundeWaehrungscode { get;set;}
    public int? KundeLandId { get;set;}
    public decimal KundeKreditlimit { get;set;}
    public bool KundeNeutralerVersand { get;set;}
    public int? LetzterBearbeiter { get;set;}
    public DateTime? ZuletztGeaendert { get;set;}

    public int? GetIdentifier() => VoreinstellungId > 0 ? VoreinstellungId : null;

    public string GetName() => Name;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "VOREINSTELLUNG_ID", VoreinstellungId },
            { "NAME", Name },
            { "KUNDE_LIEFERBEDINGUNG_ID", KundeLieferbedingungId },
            { "KUNDE_ZAHLUNGSBEDINGUNG_ID", KundeZahlungsbedingungId },
            { "KUNDE_ANREDE_ID", KundeAnredeId },
            { "KUNDE_WAEHRUNGSCODE", KundeWaehrungscode },
            { "KUNDE_LAND_ID", KundeLandId },
            { "KUNDE_KREDITLIMIT", KundeKreditlimit },
            { "KUNDE_NEUTRALER_VERSAND", KundeNeutralerVersand },
            { "LETZTER_BEARBEITER", LetzterBearbeiter },
            { "ZULETZT_GEAENDERT", ZuletztGeaendert },
        };
    }
}