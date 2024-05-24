using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using BlazorERP.Core.Utilities;

namespace BlazorERP.Components.ComponentBases;

public abstract class SearchComponentBase<TModel, TService, TFilter> : ComponentBase
 where TModel : class, new()
 where TService : IFilterOperations<TModel, TFilter>
 where TFilter : PageFilterBase, new()
{
    protected EditForm? _form;
    protected bool _isLoading;

    [Parameter]
    public TFilter Filter { get; set; } = new();
    [Inject]
    protected TService Service { get; set; } = default!;
    protected List<TModel> Data { get; set; } = [];

    [Parameter, SupplyParameterFromQuery]
    public int Page { get; set; } = 1;

    [Parameter]
    public EventCallback<TModel> OnItemSelected { get; set; }

    [Parameter]
    [Obsolete("Diese Eigenschaft ist zurzeit nicht verfügbar.")]
    public EventCallback<TModel> OnItemClicked { get; set; }

    protected int TotalItems { get; set; }
    protected int TotalPages => (int)Math.Ceiling((double)TotalItems / (double)(Filter?.Limit ?? 30));

    protected override async Task OnParametersSetAsync()
    {
        if (Page <= 0)
        {
            Page = 1;
        }

        await LoadAsync();
    }

    protected virtual IDbController GetDbController()
    {
        return new FbController();
    }
    protected virtual async Task LoadAsync(bool navigateToPage1 = false)
    {
        _isLoading = true;
        await Task.Yield();
        if (navigateToPage1)
        {
            Page = 1;
        }
        Filter.PageNumber = Page < 1 ? 1 : Page;

        using IDbController dbController = GetDbController();

        Data = await Service.GetAsync(Filter, dbController);
        TotalItems = await Service.GetTotalAsync(Filter, dbController);
        _isLoading = false;
    }
    protected virtual async Task OnPageChangedAsync(int pageNumber)
    {
        Page = pageNumber;
        await LoadAsync();
    }

}
