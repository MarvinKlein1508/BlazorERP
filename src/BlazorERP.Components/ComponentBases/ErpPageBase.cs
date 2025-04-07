using BlazorERP.Core.Modules.CoreData;
using BlazorERP.Core.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorERP.Components.ComponentBases;
public abstract class ErpPageBase : ComponentBase
{
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IDbConnectionFactory DbFactory { get; set; } = default!;
    [Inject] protected AuthService AuthService { get; set; } = default!;
    public User? CurrentUser { get; set; }
    protected override async Task OnInitializedAsync()
    {
        CurrentUser = await AuthService.GetUserAsync();
    }
}
