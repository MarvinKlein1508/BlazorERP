using BlazorERP.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddPostgresServices();

var apiService = builder.AddProject<Projects.BlazorERP_ApiService>("apiservice");

builder.AddProject<Projects.BlazorERP_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
