using BlazorERP.Core.Enums;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using BlazorERP.Core.Validators.Admin.Hilfsdaten;
using BlazorERP.Core.Validators.Stammdaten;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace BlazorERP.Components.Pages.Stammdaten.Kunden;

public partial class KundenForm
{

    private bool _showOptionsMenu;
    private KundeValidator _validator = new();

    protected override string GetListUrl() => "/Stammdaten/Kunden";
    protected override string GetEntityRedirectUrl() => $"/Stammdaten/Kunden/Edit?kundennummer={Input!.Kundennummer}";



    [Parameter, SupplyParameterFromQuery(Name = "kundennummer")]
    public override string? Identifier { get; set; }



    protected override Task BeforeSaveAsync(IDbController dbController)
    {
        if (Modus is EditMode.Create)
        {
            Input.Anlagedatum = DateTime.Now;
        }

        Input.LetzterBearbeiter = User.UserId;
        Input.ZuletztGeaendert = DateTime.Now;

        return Task.CompletedTask;
    }

    private void OnEmailChanged(string email)
    {
        Input.Email = email;
        string domain = Regex.Replace(email, @".*@", "");

        if (string.IsNullOrWhiteSpace(Input.Website))
        {
            Input.Website = domain;
        }
    }

}