using Microsoft.Extensions.DependencyInjection;

namespace NoteAPI.IntegrationTests.Helpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Remove<TService>(this IServiceCollection services)
    {
        var service = services.FirstOrDefault(x => x.ServiceType == typeof(TService));

        if (service is not null)
        {
            services.Remove(service);
        }

        return services;
    }
}