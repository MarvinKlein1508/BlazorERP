using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Sprache : IDbModelWithName<int?>
{
    public int SprachId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? GetIdentifier() => SprachId > 0 ? SprachId : null;

    public string GetName() => Name;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "SPRACH_ID", SprachId },
            { "NAME", Name },
        };
    }
}
