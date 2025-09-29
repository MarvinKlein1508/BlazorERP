using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorERP.Components.Layout;

public partial class MainNavMenu
{
    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Parameter]
    public FluentLayoutHamburger? Hamburger { get; set; }

    protected override void OnInitialized()
    {
        NavItems = new List<NavItem>
        {
            new NavItem("Home", "", "", "", []),
            new NavItem("Counter", "/counter", "", "", []),
            new NavItem("Weather", "/weather", "", "", []),
        };
    }

    public IEnumerable<NavItem> NavItems { get; private set; } = [];

    public record NavItem(string Title, string Route, string Icon, string Order, IEnumerable<NavItem> Items);

    private async Task ItemClickAsync(NavItem item)
    {
        NavigationManager.NavigateTo(item.Route);

        if (Hamburger is not null)
        {
            await Hamburger.HideAsync();
        }
    }
}
