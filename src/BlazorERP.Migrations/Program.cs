using BlazorERP.Core;
using BlazorERP.Migrations;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.RegisterServices(disableRetry: true);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
