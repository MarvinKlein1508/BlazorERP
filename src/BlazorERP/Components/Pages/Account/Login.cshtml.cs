#if DEBUG
//#define CUSTOM_LOGIN // To login as another user
#endif
using BlazorERP.Core.Enums;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using BlazorERP.Core.Options;
using BlazorERP.Core.Services;
using BlazorERP.Core.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace BlazorERP.Components.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserService _userService;
        private readonly LdapOptions _ldapOptions;

        [BindProperty]
        public LoginInput Input { get; set; } = new LoginInput();
        public string? ReturnUrl { get; set; }

        public LoginModel(UserService mitarbeiterService, IOptions<LdapOptions> ldapOptions)
        {
            _userService = mitarbeiterService;
            _ldapOptions = ldapOptions.Value;
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

                // Try LDAP login
                if (_ldapOptions.EnableLdapLogin)
                {
                    try
                    {
                        using var connection = new LdapConnection(_ldapOptions.LDAP_SERVER);

                        var networkCredential = new NetworkCredential(Input.Username, Input.Password, _ldapOptions.DOMAIN_SERVER);
                        connection.SessionOptions.SecureSocketLayer = false;
                        connection.AuthType = AuthType.Negotiate;
                        connection.Bind(networkCredential);

                        var searchRequest = new SearchRequest
                            (
                            _ldapOptions.DistinguishedName,
            $"(SAMAccountName={Input.Username})",
                            SearchScope.Subtree,
                            [
                                "cn",
                                "mail",
                                "displayName",
                                "givenName",
                                "sn",
                                "objectGUID",
                                "memberOf"
                            ]);

                        SearchResponse directoryResponse = (SearchResponse)connection.SendRequest(searchRequest);

                        SearchResultEntry searchResultEntry = directoryResponse.Entries[0];

                        Dictionary<string, string> attributes = [];
                        Guid? guid = null;

                        List<string> gruppen = [];
                        foreach (DirectoryAttribute userReturnAttribute in searchResultEntry.Attributes.Values)
                        {
                            if (userReturnAttribute.Name == "objectGUID")
                            {
                                byte[] guidByteArray = (byte[])userReturnAttribute.GetValues(typeof(byte[]))[0];
                                guid = new Guid(guidByteArray);
                                attributes.Add("guid", ((Guid)guid).ToString());
                            }
                            else if (userReturnAttribute.Name == "memberOf")
                            {
                                foreach (string item in userReturnAttribute.GetValues(typeof(string)).Cast<string>())
                                {
                                    gruppen.Add(item);
                                }
                            }
                            else
                            {
                                attributes.Add(userReturnAttribute.Name, (string)userReturnAttribute.GetValues(typeof(string))[0]);
                            }
                        }

                        attributes.TryAdd("mail", string.Empty);
                        attributes.TryAdd("sn", string.Empty);
                        attributes.TryAdd("givenName", string.Empty);
                        attributes.TryAdd("displayName", string.Empty);

                        if (guid is null)
                        {
                            throw new InvalidOperationException();
                        }

                        user = await _userService.GetAsync((Guid)guid, dbController);

                        if (user is null)
                        {
                            user = new User
                            {
                                Username = Input.Username.ToUpper(),
                                ActiveDirectoryGuid = (Guid)guid,
                                Email = attributes["mail"],
                                Firstname = attributes["givenName"],
                                Lastname = attributes["sn"],
                                IsActive = true,
                                AccountType = AccountType.ActiveDirectory
                            };

                            await _userService.CreateAsync(user, dbController);
                        }
                        else
                        {
                            // Update des User Objekts
                            user.Email = attributes["mail"];
                            user.Firstname = attributes["givenName"];
                            user.Lastname = attributes["sn"];
                            user.Username = Input.Username.ToUpper();
                            await _userService.UpdateAsync(user, dbController);
                        }

                    }
                    catch (LdapException ex)
                    {

                    }
                }

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
