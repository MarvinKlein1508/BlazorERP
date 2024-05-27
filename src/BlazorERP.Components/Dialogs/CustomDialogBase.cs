using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Utilities;
using FirebirdSql.Data.FirebirdClient;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorERP.Components.Dialogs;

public abstract class CustomDialogBase<TIdentifier, TModel, TService, TValidator> : ComponentBase, IDialogContentComponent<TModel>
    where TModel : IDbModel<TIdentifier>
    where TService : ICreateOperation<TModel>, IUpdateOperation<TModel>
    where TValidator : AbstractValidator<TModel>, new()
{
    protected EditForm? _form;
    protected TValidator _validator = new();

    [Parameter]
    public TModel Content { get; set; } = default!;

    [Inject]
    public TService Service { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    [CascadingParameter]
    public FluentDialog Dialog { get; set; } = default!;

    protected override void OnInitialized()
    {
        Dialog.Class = "modal-lg";
    }

    protected async Task SaveAsync()
    {
        if (_form is null || _form.EditContext is null)
        {
            return;
        }

        if (_form.EditContext.Validate())
        {
            using IDbController dbController = new FbController();
            await dbController.StartTransactionAsync();
            try
            {
                if (Content.GetIdentifier() is null)
                {
                    await Service.CreateAsync(Content, dbController);
                }
                else
                {
                    await Service.UpdateAsync(Content, dbController);
                }

                await dbController.CommitAsync();
            }
            catch (FbException ex)
            {
                await dbController.RollbackAsync();
                throw;
            }

            ToastService.ShowSuccess(GetSavedMessage());
            await Dialog.CloseAsync(Content);
        }
    }

    protected async Task CancelAsync()
    {
        await Dialog.CancelAsync();
    }

    protected virtual string GetSavedMessage()
    {
        return "Datensatz wurde erfolgreich gespeichert";
    }
}
