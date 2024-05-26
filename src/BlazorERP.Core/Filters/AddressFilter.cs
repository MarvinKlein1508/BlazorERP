
using BlazorERP.Core.Interfaces;

namespace BlazorERP.Core.Filters;

public class AddressFilter : PageFilterBase, IBlockedList<int>
{
    public List<int> Blocked { get; set; } = [];

    public string GetBlockedQuery()
    {
        List<string> parameterNames = [];

        for (int i = 0; i < Blocked.Count; i++)
        {
            string parameterName = $"@ADDRESS_ID{i}";
            parameterNames.Add(parameterName);
        }

        string parameterQuery = string.Join(", ", parameterNames);

        return parameterQuery;
    }
    public override Dictionary<string, object?> GetParameters()
    {
        var parameters = new Dictionary<string, object?>
        {
            { "SEARCH_PHRASE", $"%{SearchPhrase}%" }
        };

        if (Blocked.Count > 0)
        {
            for (int i = 0; i < Blocked.Count; i++)
            {
                parameters.Add($"ADDRESS_ID{i}", Blocked[i]);
            }
        }

        return parameters;
    }
}
