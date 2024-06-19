using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Filters;

public abstract class FilterBase : IDbParameterizable
{
    private string _searchPhrase = string.Empty;
    public string SearchPhrase { get => _searchPhrase; set => _searchPhrase = value?.ToUpper() ?? string.Empty; }

    public int? LanguageId { get; set; }
    public abstract Dictionary<string, object?> GetParameters();
}
