namespace BlazorERP.Core.Utilities;

public class ActivePage
{
    /// <summary>
    /// Ruft ab, welche Seite derzeit gesperrt ist.
    /// </summary>
    public required string Type { get; init; }
    /// <summary>
    /// Ruft den Namen des Moduls ab, in welchem sich die Seite befindet.
    /// </summary>
    public required string ModuleName { get; init; }
    /// <summary>
    /// Ruft den eindeutigen Identifier der Seite ab.
    /// </summary>
    public string PageId { get; init; } = string.Empty;
    /// <summary>
    /// Ruft die PersonalNummer des Mitarbeiters ab, der die Seite gerade sperrt.
    /// </summary>
    public int UserId { get; init; }
    /// <summary>
    /// Ruft den Usernamen des Benutzers ab, der die Seite derzeit sperrt.
    /// </summary>
    public string Username { get; init; } = string.Empty;
    /// <summary>
    /// Ruft den Zeitpunkt des Beginns der Sperre durch den Mitarbeiter ab.
    /// </summary>
    public DateTime Timestamp { get; private set; }
    /// <summary>
    /// Erstellt einen neuen ActivePage Datensatz
    /// </summary>
    /// <param name="type">Der typ der Seite die geseprrt werden soll</param>
    /// <param name="pageId">Der Identifier der Seite. Z.B. die Auftragsnummer, oder die Bestellnummer</param>
    /// <param name="userId">Die Mitarbeiter ID</param>
    public ActivePage()
    {
        Timestamp = DateTime.Now;
    }
    /// <summary>
    /// Aktualisiert den <see cref="Timestamp"/> auf die aktuelle Zeit.
    /// </summary>
    public void UpdateTimestamp()
    {
        Timestamp = DateTime.Now;
    }
}
