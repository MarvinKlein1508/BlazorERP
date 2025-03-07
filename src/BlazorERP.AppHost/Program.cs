using BlazorERP.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

bool testOnly = false;

foreach (var arg in args)
{
    if (arg.StartsWith("--testonly"))
    {
        testOnly = true;
    }
}

var (db, migrationSvc) = builder.AddPostgresServices(testOnly: testOnly);

builder.AddProject<Projects.BlazorERP_Web>("webfrontend")
    .WithReference(db)
    .WaitForCompletion(migrationSvc)
    .WithExternalHttpEndpoints();

builder.Build().Run();
