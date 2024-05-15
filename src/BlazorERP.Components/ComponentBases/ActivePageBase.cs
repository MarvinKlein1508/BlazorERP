using BlazorERP.Core.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorERP.Components.ComponentBases;

public abstract class ActivePageBase : ComponentBase, IAsyncDisposable
{
    // TODO: Logik überarbeiten, dass die Klasse auch zum Tracken von Aktiven Seiten genutzt werden kann
    public ActivePage? Page { get; set; }
    public ActivePage? BlockedPage { get; set; }

    protected bool IsPageBlockedByOtherUser { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    private DotNetObjectReference<ActivePageBase> _reference;
    private IJSObjectReference? _jsModule;

    public ActivePageBase()
    {
        _reference = DotNetObjectReference.Create(this);
    }

    public abstract Task CheckActivePageAsync();
    protected override async Task OnInitializedAsync()
    {
        _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./assets/js/activepage.js");

        if (_jsModule is not null)
        {
            await _jsModule.InvokeVoidAsync("init", _reference);
        }

        await base.OnInitializedAsync();
    }

    public virtual async ValueTask DisposeAsync()
    {
        if (_jsModule is not null)
        {
            await _jsModule.DisposeAsync();
        }

        ActivePages.RemoveActivePage(Page);
    }


    [JSInvokable]
    public void OnBeforePageUnload()
    {
        ActivePages.RemoveActivePage(Page);
    }
}
