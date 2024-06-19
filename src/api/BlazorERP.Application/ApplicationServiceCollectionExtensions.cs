using BlazorERP.Application.Database;
using BlazorERP.Application.Repositories;
using BlazorERP.Application.Repositories.Interfaces;
using BlazorERP.Application.Services;
using BlazorERP.Application.Services.Interfaces;
using Dapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorERP.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUserService, UserService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new FirebirdConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>();
        return services;
    }

    public static IServiceCollection ConfigureDapper(this IServiceCollection services)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        return services;
    }
}
