#if DEBUG
//#define CUSTOM_LOGIN // To login as another user
#endif
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using BlazorERP.Core.Services;
using BlazorERP.Core.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Web;

namespace BlazorERP.Components.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserService _userService;

        [BindProperty]
        public LoginInput Input { get; set; } = new LoginInput();
        public string? ReturnUrl { get; set; }

        public LoginModel(UserService mitarbeiterService)
        {
            _userService = mitarbeiterService;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = GetReturnUrl(returnUrl);

            try
            {
                // Clear the existing external cookie
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch { }

        }


        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl = GetReturnUrl(returnUrl);

            if (ModelState.IsValid)
            {
                using IDbController dbController = new FbController();


                User? user = await _userService.GetByUsernameAsync(Input.Username, dbController);

                if (user is not null)
                {
                    PasswordHasher<User> hasher = new();

                    string passwordHashed = hasher.HashPassword(user, Input.Password + user.Salt);

                    PasswordVerificationResult result = hasher.VerifyHashedPassword(user, user.Password, Input.Password + user.Salt);
                    // We handle login whenever we have a user object. So when it fails we need to set it to null
                    if (result is PasswordVerificationResult.Failed)
                    {
                        user = null;
                    }
                }

                // TODO: Try LDAP login

                if (user is not null)
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, user.GetName()),
                        new("userId", user.UserId.ToString()),

                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = Input.RememberMe
                    };


                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("login-error", "Username oder Passwort ist falsch.");
                }
            }
            return Page();
        }

        private string GetReturnUrl(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return Url.Content("~/");
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
                        url += $"/{HttpUtility.UrlEncode(item)}";
                    }
                }

                return url.Replace("//", "/");
            }
        }
    }

    public class LoginInput
    {
        [Required]
        public string Username { get; set; } = String.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = String.Empty;
        [Display(Name = "Angemeldet bleiben")]
        public bool RememberMe { get; set; }
    }
}
