using BlazorERP.Core;

namespace BlazorERP.AppHost;

public static class PostgresExtensions
{
  
    public static (IResourceBuilder<PostgresDatabaseResource> db, IResourceBuilder<ProjectResource> migrationSvc) AddPostgresServices(this IDistributedApplicationBuilder builder, bool testOnly = false)
    {
        var dbServer = builder.AddPostgres("database", port: 50000)
            .WithImageTag(Constants.Postgres.Versions.POSTGRES);

        if (!testOnly)
        {
            dbServer = dbServer.WithLifetime(ContainerLifetime.Persistent)
                .WithDataVolume($"{Constants.Postgres.DBNAME}-data", false)
                .WithPgAdmin(config =>
                {
                    config.WithImageTag(Constants.Postgres.Versions.PGADMIN);
                    config.WithLifetime(ContainerLifetime.Persistent);
                });
        }
        else
        {
            dbServer = dbServer
                .WithLifetime(ContainerLifetime.Session);
        }

        var outDb = dbServer.AddDatabase(Constants.Postgres.DBNAME);

        var migrationSvc = builder.AddProject<Projects.BlazorERP_Migrations>($"{Constants.Postgres.DBNAME}migrationsvc")
            .WithReference(outDb)
            .WaitFor(dbServer);

        return (outDb, migrationSvc);
    }
}
