using BlazorERP.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var (db, migrationSvc) = builder.AddPostgresServices();

builder.AddProject<Projects.BlazorERP_Web>("webfrontend")
    .WithReference(db)
    .WaitForCompletion(migrationSvc)
    .WithExternalHttpEndpoints();

builder.Build().Run();
