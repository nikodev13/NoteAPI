using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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
        });
        
        builder.UseEnvironment("Development");
    }
}