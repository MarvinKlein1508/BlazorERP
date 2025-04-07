using BlazorERP.Components;
using BlazorERP.Core;
using BlazorERP.Core.Extensions;
using BlazorERP.Core.Modules.CoreData;
using BlazorERP.Core.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor()
             .AddCircuitOptions(options =>
             {
                 options.DetailedErrors = true;
             });

builder.Services.AddHttpContextAccessor();

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Languages";
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddSingleton<UserService>();
builder.Services.AddScoped<AuthService>();

string connectionString = config.GetConnectionString("Default") ?? throw new NullReferenceException("No default connection string provided");
builder.Services.AddDatabase(connectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var accountGroup = app.MapGroup("/Account");

accountGroup.MapGet("/Logout", async (HttpContext context) =>
{
    //if (context.User.Identity is not null)
    //{
    //    Log.Information("Logout: {username}", context.User.Identity.Name);
    //}

    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.LocalRedirect("/Account/Login");
});

app.MapControllers();

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(Constants.SupportedCultures[0].Name)
    .AddSupportedCultures(Constants.SupportedCulturesCodes)
    .AddSupportedUICultures(Constants.SupportedCulturesCodes);

app.UseRequestLocalization(localizationOptions);

app.Run();
