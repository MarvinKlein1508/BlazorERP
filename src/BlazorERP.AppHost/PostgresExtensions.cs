using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorERP.AppHost;

public static class PostgresExtensions
{
    public static class VERSIONS
    {
        public const string POSTGRES = "17.2";
        public const string PGADMIN = "latest";
    }

    public static class CONSTANTS
    {
        public const string DBNAME = "BlazorERP";
    }

    public static void AddPostgresServices(this IDistributedApplicationBuilder builder, bool testOnly = false)
    {
        var dbServer = builder.AddPostgres("database")
            .WithImageTag(VERSIONS.POSTGRES);

        if (!testOnly)
        {
            dbServer = dbServer.WithLifetime(ContainerLifetime.Persistent)
                .WithDataVolume($"{CONSTANTS.DBNAME}-data", false)
                .WithPgAdmin(config =>
                {
                    config.WithImageTag(VERSIONS.PGADMIN);
                    config.WithLifetime(ContainerLifetime.Persistent);
                });
        }
        else
        {
            dbServer = dbServer
                .WithLifetime(ContainerLifetime.Session);
        }

        var outDb = dbServer.AddDatabase(CONSTANTS.DBNAME);

        //var migrationSvc = builder.AddProject<Projects.SharpSite_Data_Postgres_Migration>($"{SharpSite.Data.Postgres.Constants.DBNAME}migrationsvc")
        //    .WithReference(outDb)
        //    .WaitFor(dbServer);
    }
}
