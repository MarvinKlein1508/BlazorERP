using System.Security.Claims;
using System.Text.Json;
using BlazorERP.Pages.Account.Pages;
using BlazorERP.Pages.Account.Pages.Manage;
using BlazorERP.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace BlazorERP.Pages.Account;

internal static class IdentityComponentsEndpointRouteBuilderExtensions
{
    // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/Account");

        accountGroup.MapPost("/Logout", async (
             ClaimsPrincipal user,
             [FromServices] SignInManager<ApplicationUser> signInManager,
             [FromForm] string returnUrl) =>
         {
             await signInManager.SignOutAsync();
             return TypedResults.LocalRedirect($"~/{returnUrl}");
         });

        var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

        return accountGroup;
    }
}
