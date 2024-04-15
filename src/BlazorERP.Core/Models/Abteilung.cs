using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Abteilung : IDbModelWithName<int?>
{
    public int AbteilungId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ActiveDirectoryGroupCN { get; set; } = string.Empty;

    public int? GetIdentifier() => AbteilungId <= 0 ? null : AbteilungId;


    public string GetName() => Name;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "ABTEILUNG_ID", AbteilungId },
            { "NAME", Name },
            { "ACTIVE_DIRECTORY_GROUP_CN", ActiveDirectoryGroupCN },
        };
    }
}
