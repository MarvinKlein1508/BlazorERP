using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorERP.Components.Layout;

public partial class MainNavMenu
{
    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Parameter]
    public FluentLayoutHamburger? Hamburger { get; set; }
}
