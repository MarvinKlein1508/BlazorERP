using BlazorERP.Core.Enums;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using BlazorERP.Core.Services;
using BlazorERP.Core.Utilities;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorERP.Core.ComponentBases;

public abstract class EditPageBase<TIdentifier, TModel, TService> : ActivePageBase
    where TModel : class, IDbModel<TIdentifier>, new()
    where TService : IModelService<TModel, TIdentifier>
{
    protected EditForm? _form;

    [Parameter, SupplyParameterFromQuery]
    public virtual TIdentifier? Identifier { get; set; }

    protected TModel? Input { get; set; }

    protected EditMode Modus { get; set; } = EditMode.Create;

    [Inject]
    protected TService Service { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    protected AuthService AuthService { get; set; } = default!;

    [Inject]
    protected IToastService ToastService { get; set; } = default!;
    [Inject]
    protected IMessageService MessageService { get; set; } = default!;

    [Inject]
    protected IDialogService DialogService { get; set; } = default!;

    public User? User { get; set; }
    protected string FinalBreadcrumbItemName { get; set; } = string.Empty;


    protected bool _isLoading;

    protected override async Task OnInitializedAsync()
    {
        User = await AuthService.GetUserAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        using IDbController dbController = new FbController();

        if (Identifier is not null)
        {
            await LoadEditModeAsync(dbController);
            await CheckActivePageAsync();
        }
        else
        {
            Input = new TModel();
            FinalBreadcrumbItemName = "Neu";
            //EditEnabled = true;
            Modus = EditMode.Create;
            await InitializeModelAsync(true, dbController);
        }


    }

    protected async Task SaveAsync()
    {
        if (_form is null || _form.EditContext is null || Input is null)
        {
            return;
        }
        _isLoading = true;
        if (_form.EditContext.Validate())
        {
            using IDbController dbController = new FbController();
            await dbController.StartTransactionAsync();
            try
            {
                await BeforeSaveAsync(dbController);

                if (Modus is EditMode.Create)
                {
                    await Service.CreateAsync(Input, dbController);
                }
                else
                {
                    await Service.UpdateAsync(Input, dbController);
                }

                await OnSaveAsync(dbController);

                await dbController.CommitAsync();

                Storage.UpdateStorage<TModel, TIdentifier>(Input);
            }
            catch (FbException ex)
            {
                await dbController.RollbackAsync();
                throw;
            }

            
            
            NavigationManager.NavigateTo(GetEntityRedirectUrl());
            ToastService.ShowSuccess("Datensatz wurde erfolgreich gespeichert");
            
            await OnParametersSetAsync();
        }
        _isLoading = false;
    }

    protected async Task LoadEditModeAsync(IDbController dbController)
    {
        var entry = await Service.GetAsync(Identifier!, dbController);
        if (entry is not null)
        {
            Input = entry.DeepCopyByExpressionTree();
            //EditEnabled = true; // TODO: Refactor
            Modus = EditMode.Edit;
            if (Modus is EditMode.Edit && entry is IDbModelWithName<TIdentifier> modelWithName)
            {
                FinalBreadcrumbItemName = modelWithName.GetName();
            }
            else
            {
                FinalBreadcrumbItemName = entry.GetIdentifier()!.ToString()!;
            }

            await InitializeModelAsync(false, dbController);
        }
    }

    /// <summary>
    /// This method can be used to run more update logic during the transaction.
    /// </summary>
    /// <param name="dbController"></param>
    /// <returns></returns>
    protected virtual Task OnSaveAsync(IDbController dbController)
    {
        return Task.CompletedTask;
    }

    protected virtual Task BeforeSaveAsync(IDbController dbController)
    {
        return Task.CompletedTask;
    }

    protected virtual Task BeforeDeleteAsync(IDbController dbController)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnDeleteAsync(IDbController dbController)
    {
        return Task.CompletedTask;
    }


    protected virtual Task InitializeModelAsync(bool newEntry, IDbController dbController)
    {
        return Task.CompletedTask;
    }
    /// <summary>
    /// This method should return a URL to open an entity from the database.
    /// <para>
    /// For example: /Admin/Users/Edit?userId={Identifier}
    /// </para>
    /// </summary>
    /// <returns></returns>
    protected abstract string GetEntityRedirectUrl();
    /// <summary>
    /// Gibt die Url zur Umleitung an die Liste an.
    /// <para>
    /// Beispiel: /Admin/Users
    /// </para>
    /// </summary>
    /// <returns></returns>
    protected abstract string GetListUrl();

    public override Task CheckActivePageAsync()
    {
        if (User is not null)
        {
            var (success, page) = ActivePages.AddActivePage(new ActivePage
            {
                Type = GetEntityRedirectUrl(),
                PageId = Identifier?.ToString() ?? string.Empty,
                ModuleName = string.Empty,
                UserId = User.UserId,
                Username = User.GetName()
            });

            if (success)
            {
                Page = page;
                BlockedPage = null;
            }
            else
            {
                BlockedPage = page;
                Page = null;

            }

            IsPageBlockedByOtherUser = !success;
        }
        return Task.CompletedTask;
    }

    protected virtual async Task ShowDeleteModalAsync()
    {

        var dialog = await DialogService.ShowConfirmationAsync("Möchten Sie diesen Datensatz wirklich löschen?", "Löschen", "Abbrechen", "Datensatz löschen?");
        var result = await dialog.Result;

        if(!result.Cancelled)
        {

            using IDbController dbController = new FbController();
            await dbController.StartTransactionAsync();
            try
            {
                await BeforeDeleteAsync(dbController);
                await Service.DeleteAsync(Input, dbController);
                await OnDeleteAsync(dbController);
                await dbController.CommitAsync();

                Storage.DeleteFromStorage<TModel, TIdentifier>(Input);
            }
            catch (FbException ex)
            {
                await dbController.RollbackAsync();
                throw;
            }
            

            MessageService.ShowMessageBar(options =>
            {
                options.Title = "Datensatz wurde erfolgreich gelöscht!";
                options.Intent = MessageIntent.Success;
                options.Section = Constants.MESSAGEBAR_TOP_SECTION;
                options.Timeout = 4000;
            });

            NavigationManager.NavigateTo(GetListUrl());
        }
    }

}
