using BlazorERP.Core.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorERP.Core.ComponentBases;

public abstract class EditPageBase<TIdentifier>
{
    protected EditForm? _form;

    [Parameter]
    public TIdentifier Identifier { get; set; }

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


    protected virtual Task InitializeModelAsync(bool newEntry, IDbController dbController)
    {
        return Task.CompletedTask;
    }

}
