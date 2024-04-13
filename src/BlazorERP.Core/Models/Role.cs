using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Role : IDbModelWithName<int?>
{
    public int RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ActiveDirectoryGroupCN { get; set; } = string.Empty;

    public int? GetIdentifier() => RoleId <= 0 ? null : RoleId;


    public string GetName() => Name; 

    public Dictionary<string, object?> GetParameters()
    {
        throw new NotImplementedException();
    }
}
