using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NoteAPI.Persistence;
using NoteAPI.Services;
using UserContextService = NoteAPI.IntegrationTests.Services.UserContextService;

namespace NoteAPI.IntegrationTests.Helpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Remove<IUserContextService>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
            
            // sqlite db
            services.Remove<DbContextOptions<NoteDbContext>>();
            services.Remove<NoteDbContext>();
            services.AddSingleton<DbContextOptions<NoteDbContext>>(
                x => new DbContextOptionsBuilder<NoteDbContext>().UseSqlite("Data source=test.db3").Options);
            services.AddDbContext<NoteDbContext, SqliteNoteDbContext>(ServiceLifetime.Singleton);
        });
        
        builder.UseEnvironment("Development");
    }
}