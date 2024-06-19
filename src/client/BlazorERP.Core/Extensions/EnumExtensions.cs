using BlazorERP.Core.Enums;

namespace BlazorERP.Core.Extensions;

public static class EnumExtensions
{
    public static string ToText(this AccountType accountType) => accountType switch
    {
        AccountType.LocalAccount => "Lokales Konto",
        AccountType.ActiveDirectory => "Active Directory",
        _ => "Unbekannt",
    };
}
