using System.Reflection;

namespace NoteAPI.Shared.Endpoints;

public static class EndpointsBuilder
{
    public static IEndpointRouteBuilder RegisterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var assembly = Assembly.GetExecutingAssembly();
        assembly.GetTypes()
            .Where(x => x.IsClass && x.IsAssignableTo(typeof(IEndpoint)))
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>()
            .ToList()
            .ForEach(x => x!.Configure(endpoints));
        

        return endpoints;
    }

    public static IServiceCollection RegisterEndpointsHandlers(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        assembly.GetTypes()
            .Where(x => x.IsClass)
            .Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType 
                && (@interface.GetGenericTypeDefinition() == typeof(IRequestHandler<>))))
            .ToList()
            .ForEach(x => services.AddScoped(x));
       
        return services;
    }
}
