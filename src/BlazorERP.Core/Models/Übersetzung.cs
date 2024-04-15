using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Übersetzung : IDbParameterizable
{
    public string Code { get; set; } = string.Empty;
    public int? SprachId { get; set; }
    public int ParentId { get; set; }
    public string ValueText { get; set; } = string.Empty;
    public string ValueBlob { get; set; } = string.Empty;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "CODE", Code },
            { "SPRACH_ID", SprachId },
            { "PARENT_ID", ParentId },
            { "VALUE_TEXT", ValueText },
            { "VALUE_BLOB", ValueBlob },
        };
    }
}


