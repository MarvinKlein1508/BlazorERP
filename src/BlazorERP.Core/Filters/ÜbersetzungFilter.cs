
namespace BlazorERP.Core.Filters;

public class ÜbersetzungFilter : PageFilterBase
{
    public override Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "SEARCH_PHRASE", $"%{SearchPhrase}%" }
        };
    }
}
