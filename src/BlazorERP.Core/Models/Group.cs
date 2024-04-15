using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Group : IDbModelWithName<int?>
{
    public int GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ActiveDirectoryGroupCN { get; set; } = string.Empty;

    public int? GetIdentifier() => GroupId <= 0 ? null : GroupId;


    public string GetName() => Name; 

    public Dictionary<string, object?> GetParameters()
    {
        throw new NotImplementedException();
    }
}
