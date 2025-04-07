using System.Globalization;

namespace BlazorERP.Core;
public static class Constants
{
    public static CultureInfo[] SupportedCultures =>
    [
        new CultureInfo("en"),
        new CultureInfo("de"),
    ];

    public static string[] SupportedCulturesCodes =>
    [
        "en",
        "de",
    ];
}
