using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlazorERP.Core;

public static class IHostApplicationBuilderExtenions
{
    public static IHostApplicationBuilder RegisterServices(this IHostApplicationBuilder builder, bool disableRetry = false)
    {
        builder.AddNpgsqlDbContext<AppDbContext>(Constants.Postgres.DBNAME, configure =>
        {
            configure.DisableRetry = disableRetry;
        });

        return builder;
    }
}
