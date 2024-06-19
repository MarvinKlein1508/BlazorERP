
namespace BlazorERP.Core.Filters;

public class CostCenterFilter : PageFilterBase
{
    public override Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "SEARCH_PHRASE", $"%{SearchPhrase}%" }
        };
    }
}