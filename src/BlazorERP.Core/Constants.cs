using System.Reflection;

namespace BlazorERP.Core;

public static class Constants
{
    public const string MESSAGEBAR_TOP_SECTION = "MESSAGES_TOP";

    public static string GetVersion()
    {
        return Assembly.GetEntryAssembly()!.GetName()!.Version!.ToString(3);
    }
}
