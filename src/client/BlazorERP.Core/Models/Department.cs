using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models.Abstract;

namespace BlazorERP.Core.Models;

public class Department : TranslationBase, IDbModel<int?>
{
    public int DepartmentId { get; set; }
    public string ActiveDirectoryGroupCN { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }

    public int? GetIdentifier() => DepartmentId <= 0 ? null : DepartmentId;

    public string CreatedByName { get; set; } = string.Empty;
    public string LastModifiedName { get; set; } = string.Empty;


    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "DEPARTMENT_ID", DepartmentId },
            { "ACTIVE_DIRECTORY_GROUP_CN", ActiveDirectoryGroupCN },
            { "CREATED_AT", CreatedAt },
            { "CREATED_BY", CreatedBy },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModified },
        };
    }
}
