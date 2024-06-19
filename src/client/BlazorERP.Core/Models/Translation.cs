using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Translation : IDbParameterizable
{
    public string Code { get; set; } = string.Empty;
    public int? LanguageId { get; set; }
    public int ParentId { get; set; }
    public string ValueText { get; set; } = string.Empty;
    public string ValueBlob { get; set; } = string.Empty;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "CODE", Code },
            { "LANGUAGE_ID", LanguageId },
            { "PARENT_ID", ParentId },
            { "VALUE_TEXT", ValueText },
            { "VALUE_BLOB", ValueBlob },
        };
    }
}


