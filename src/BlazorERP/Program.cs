using BlazorERP.Components;
using BlazorERP.Core.Models;
using BlazorERP.Core.Services;
using BlazorERP.Core.Utilities;
using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// will allow dapper to parse columns like user_id to UserId
DefaultTypeMap.MatchNamesWithUnderscores = true;





// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages().WithRazorPagesRoot("/Components/Pages");

builder.Services.AddServerSideBlazor()
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 1024 * 1024)
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

builder.Services.AddFluentUIComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AbteilungService>();
builder.Services.AddScoped<AuthService>();


FbController.Initialize(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();