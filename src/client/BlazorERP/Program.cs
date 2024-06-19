using Blazored.LocalStorage;
using BlazorERP;
using BlazorERP.Components;
using BlazorERP.Components.Infrastructure;
using BlazorERP.Core.Models;
using BlazorERP.Core.Options;
using BlazorERP.Core.Services;
using BlazorERP.Core.Utilities;
using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// will allow dapper to parse columns like user_id to UserId
DefaultTypeMap.MatchNamesWithUnderscores = true;
SqlMapper.AddTypeHandler(typeof(Guid), new GuidTypeHandler());
SqlMapper.RemoveTypeMap(typeof(Guid));
SqlMapper.RemoveTypeMap(typeof(Guid?));


builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true,
    };
});

builder.Services.AddBlazoredLocalStorage();

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
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CacheStorageAccessor>();


// Options
builder.Services.AddOptions<LdapOptions>()
    .Bind(config.GetRequiredSection(LdapOptions.SectionName));


//builder.Services.AddScoped<CustomAuthenticationStateProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<CustomAuthenticationStateProvider>());

builder.Services.AddHttpClient("ApiClient")
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiServer"] ?? "");
    });


FbController.Initialize(config);
await Storage.InitAsync(config);
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