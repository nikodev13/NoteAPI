namespace NoteAPI.Services;

public static class Extensions
{
    public static IServiceCollection RegisterAppServices(this IServiceCollection services)
    {
        services.AddScoped<IUserContextService, UserContextService>();

        return services;
    }
}