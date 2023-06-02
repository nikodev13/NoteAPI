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
            services.Remove<DbContextOptions<NoteDbContextSQLLite>>();
            services.AddDbContext<NoteDbContextSQLLite>(x => x.UseSqlite());
        });
        
        builder.UseEnvironment("Development");
    }
}