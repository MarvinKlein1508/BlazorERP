using BlazorERP.Core.Modules.CoreData;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using System.Web;

namespace BlazorERP.Components.Pages.Account;
public partial class Login
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [SupplyParameterFromForm] public LoginInput Input { get; set; } = new();
    [SupplyParameterFromQuery] public string ReturnUrl { get; set; } = "/";

    protected override async Task OnInitializedAsync()
    {
        ReturnUrl = GetReturnUrl(ReturnUrl);

        if (HttpContextAccessor.HttpContext is not null && HttpMethods.IsGet(HttpContextAccessor.HttpContext.Request.Method))
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    private async Task HandleLoginAsync()
    {
        await Task.Delay(1);
    }

    private string GetReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return "/";
        }
        else
        {
            string[] parts = returnUrl.Split('/');

            string url = string.Empty;

            foreach (var item in parts)
            {
                var parameter = item.Split('?');
                if (parameter.Length == 2)
                {
                    url += $"/{HttpUtility.UrlEncode(parameter[0])}";
                    url += $"?{parameter[1]}";
                }
                else
                {
                    url += $"/{item}";
                }
            }

            return url.Replace("//", "/");
        }

    }
}