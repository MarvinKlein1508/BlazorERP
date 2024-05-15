using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorERP.Components.ComponentBases;

public sealed class CustomDialogParameters : DialogParameters
{
    public string Text { get; set; } = string.Empty;
}
