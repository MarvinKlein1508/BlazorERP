using Microsoft.Extensions.DependencyInjection;

namespace BlazorERP.Components.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add common client services required by the BlazorERP project
    /// </summary>
    /// <param name="services">Service collection</param>
    public static IServiceCollection AddBlazorERPServices(this IServiceCollection services)
    {
        services.AddSingleton<MainNavProvider>();

        return services;
    }
}
