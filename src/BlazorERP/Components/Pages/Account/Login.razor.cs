using BlazorERP.Core.Modules.CoreData;
using BlazorERP.Core.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Web;
using Localizer = BlazorERP.Languages.Login;

namespace BlazorERP.Components.Pages.Account;
public partial class Login
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
    [Inject] private UserService UserService { get; set; } = default!;
    [Inject] private IDbConnectionFactory DbFactory { get; set; } = default!;

    [SupplyParameterFromForm] public LoginInput Input { get; set; } = new();
    [SupplyParameterFromQuery] public string ReturnUrl { get; set; } = "/";

    private string? _errorMessage;
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
        using var connection = await DbFactory.CreateConnectionAsync();

        var user = await UserService.GetAsync(Input.Username, AccountType.LocalAccount, connection);

        if (user is not null && user.Password is not null)
        {
            PasswordHasher<User> hasher = new();
            string passwordHashed = hasher.HashPassword(user, Input.Password + user.Salt);

            PasswordVerificationResult result = hasher.VerifyHashedPassword(user, user.Password, Input.Password + user.Salt);
            
            if (result is PasswordVerificationResult.Failed)
            {
                user = null;
            }
        }
        else
        {
            user = null;
        }


        if (user is not null)
        {
            
            var claims = new List<Claim>
            {
                new("userId", user.UserId.ToString()),
                new(ClaimTypes.Name, user.FirstName)
                        
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe
            };

            if (HttpContextAccessor.HttpContext is null)
            {
                _errorMessage = Localizer.ERROR_HTTPCONTEXT_IS_NULL;
                return;
            }

            await HttpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            Navigation.NavigateTo(ReturnUrl ?? "/");
        }
        else
        {
            _errorMessage = Localizer.ERROR_INVALID_LOGIN;
        }
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