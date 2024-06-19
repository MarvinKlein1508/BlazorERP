namespace BlazorERP.Core.Utilities;

public static class ActivePages
{
    /// <summary>
    /// Ruft alle derzeit gesperrten Seiten ab.
    /// </summary>
    public readonly static List<ActivePage> Pages = [];
    /// <summary>
    /// Fügt eine <see cref="ActivePage"/> zur Liste der gesperrten Seiten hinzu.
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static (bool success, ActivePage page) AddActivePage(ActivePage page)
    {
        var suchePage = Pages.FirstOrDefault(x => x.Type == page.Type && x.PageId == page.PageId);

        if (suchePage is null)
        {
            Pages.Add(page);
            return (true, page);
        }

        // Wenn die UserId identisch ist, dann darf der User die Seite öffnen
        if (suchePage.UserId == page.UserId)
        {
            // Damit die Sperre nicht nach 10 Minuten ausläuft, erhöhen wir die Sperrzeit.
            suchePage.UpdateTimestamp();
            return (true, suchePage);
        }

        // Wenn der Browser gesschlossen wird, dann ist die Seite noch gesperrt, daher geben wir automatisch nach 10 Minuten alles wieder frei
        if (suchePage.Timestamp.AddMinutes(10) < DateTime.Now)
        {
            RemoveActivePage(suchePage);
            Pages.Add(page);
            return (true, page);
        }


        return (false, suchePage);
    }
    /// <summary>
    /// Entfernt eine <see cref="ActivePage"/> aus den gesperrten Seiten.
    /// </summary>
    /// <param name="page"></param>
    public static void RemoveActivePage(ActivePage? page)
    {
        if (page is not null)
        {
            Pages.Remove(page);
        }
    }
}
