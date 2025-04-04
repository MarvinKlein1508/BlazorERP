using BlazorERP.Core.Utilities;
using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorERP.Core.Extensions;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        // Configure Dapper
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddSingleton<IDbConnectionFactory>(_ => new PostgresConnectionFactory(connectionString));
        return services;
    }
}
