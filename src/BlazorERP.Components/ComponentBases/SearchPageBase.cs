using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using FirebirdSql.Data.Services;
using BlazorERP.Core.Utilities;
using Microsoft.AspNetCore.WebUtilities;
using System;
using Microsoft.Extensions.Primitives;

namespace BlazorERP.Components.ComponentBases;

public abstract class SearchPageBase<TModel, TService, TFilter> : ComponentBase
    where TModel : class, new()
    where TService : IFilterOperations<TModel, TFilter>
    where TFilter : PageFilterBase, new()
{
    protected EditForm? _form;
    protected bool _isLoading;

    [Parameter, SupplyParameterFromForm]
    public TFilter Filter { get; set; } = new();
    [Inject]
    protected TService Service { get; set; } = default!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    protected List<TModel> Data { get; set; } = [];

    [Parameter, SupplyParameterFromQuery]
    public int Page { get; set; } = 1;

    protected int TotalItems { get; set; }
    protected int TotalPages => (int)Math.Ceiling((double)TotalItems / (double)(Filter?.Limit ?? 30));

    protected Uri CurrentUri => NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
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
            var parameters = GetQueryParameter();
            parameters.Remove("page");
            parameters.Add("page", "1");

            NavigationManager.NavigateTo($"{CurrentUri.AbsolutePath}?{BuildQueryParameters(parameters)}");
        }
        Filter.PageNumber = Page < 1 ? 1 : Page;

        using IDbController dbController = GetDbController();

        Data = await Service.GetAsync(Filter, dbController);
        TotalItems = await Service.GetTotalAsync(Filter, dbController);
        _isLoading = false;
    }
    protected virtual Task OnPageChangedAsync(int pageNumber)
    {
        var parameters = GetQueryParameter();
        parameters.Remove("page");
        parameters.Add("page", pageNumber.ToString());


        NavigationManager.NavigateTo($"{CurrentUri.AbsolutePath}?{BuildQueryParameters(parameters)}");

        return Task.CompletedTask;
    }

    protected Dictionary<string, StringValues> GetQueryParameter() => QueryHelpers.ParseQuery(CurrentUri.Query);
    protected string BuildQueryParameters(Dictionary<string, StringValues> parameters)
    {
        if (parameters.Count == 0)
        {
            return string.Empty;
        }

        var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p!.Value)}"));
        return queryString;
    }
}
