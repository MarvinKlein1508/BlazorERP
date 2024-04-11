using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;

namespace BlazorERP.Core.ComponentBases;

public abstract class SearchPageBase<TModel, TService, TFilter> : ComponentBase
    where TModel : class, new()
    where TService : IFilterOperations<TModel, TFilter>
    where TFilter : PageFilterBase, new()
{
    protected EditForm? _form;
    protected bool _isLoading;

    [Parameter]
    public string BaseUrl { get; set; } = string.Empty;
    protected abstract bool InternalNavigation { get; }
    [Parameter, SupplyParameterFromForm]
    public TFilter? Filter { get; set; } = new();
    [Inject]
    protected TService Service { get; set; } = default!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    protected List<TModel> Data { get; set; } = [];

    [Parameter, SupplyParameterFromQuery]
    public int Page { get; set; } = 1;

    protected int TotalItems { get; set; }
    protected int TotalPages => (int)Math.Ceiling((double)TotalItems / (double)(Filter?.Limit ?? 30));

    protected override async Task OnParametersSetAsync()
    {
        if (Page <= 0)
        {
            Page = 1;
        }

        if (Filter is not null)
        {
            await LoadAsync();
        }
    }

    protected abstract IDbController GetDbController();
    protected virtual async Task LoadAsync()
    {
        if (Filter is null)
        {
            return;
        }

        Data.Clear();
        _isLoading = true;
        await Task.Yield();

        Filter.PageNumber = Page < 1 ? 1 : Page;



        using IDbController dbController = GetDbController();

        Data = await Service.GetAsync(Filter, dbController);
        TotalItems = await Service.GetTotalAsync(Filter, dbController);
        _isLoading = false;
    }
    protected virtual async Task OnPageChangedAsync(int pageNumber)
    {
        if (InternalNavigation)
        {
            Page = pageNumber;
            await LoadAsync();
        }
        else if (pageNumber != Page)
        {
            NavigationManager.NavigateTo($"{BaseUrl}?page={pageNumber}");
        }
        else
        {
            await LoadAsync();
        }
    }
}
