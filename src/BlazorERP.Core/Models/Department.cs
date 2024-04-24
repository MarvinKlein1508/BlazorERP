using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Models;

public class Department : IDbModel<int?>
{
    public int DepartmentId { get; set; }
    public string ActiveDirectoryGroupCN { get; set; } = string.Empty;

    public int? LastModifiedBy { get; set; }
    public DateTime LastModified { get; set; }

    public int? GetIdentifier() => DepartmentId <= 0 ? null : DepartmentId;

    public List<Translation> Translations { get; set; } = [];

    public string GetName(int languageId)
    {
        var translation = Translations.First(x => x.LanguageId == languageId);
        return translation.ValueText;
    }

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "DEPARTMENT_ID", DepartmentId },
            { "ACTIVE_DIRECTORY_GROUP_CN", ActiveDirectoryGroupCN },
            { "LAST_MODIFIED_BY", LastModifiedBy },
            { "LAST_MODIFIED", LastModifiedBy },
        };
    }
}
