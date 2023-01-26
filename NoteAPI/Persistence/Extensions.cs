using Microsoft.EntityFrameworkCore;

namespace NoteAPI.Persistence;

public static class Extensions
{
    public static IServiceCollection ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NoteDbContext>(
            options => options.UseSqlServer(configuration.GetConnectionString("NoteDbConnectionString")));
        
        return services;
    }
}