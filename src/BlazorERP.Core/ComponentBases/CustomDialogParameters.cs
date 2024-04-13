using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorERP.Core.ComponentBases;

public sealed class CustomDialogParameters : DialogParameters
{
    public string Text { get; set; } = string.Empty;
}
