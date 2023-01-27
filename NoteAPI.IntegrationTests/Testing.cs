using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NoteAPI.IntegrationTests.Helpers;
using NoteAPI.Persistence;
using NoteAPI.Shared.Endpoints;
using Xunit;

namespace NoteAPI.IntegrationTests;

[CollectionDefinition(TestingCollection.Name)]
public class TestingCollection : ICollectionFixture<Testing>
{
    public const string Name = "Testing";
}

public class Testing : IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HttpClient _client;

    public Testing()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    public HttpClient GetClient()
    {
        return _client;
    }

    public async ValueTask<List<TEntity>> FindEntities<TEntity>(Expression<Func<TEntity, bool>> selector) where TEntity : class
    {
        var service = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<NoteDbContext>();
        return await service.Set<TEntity>().Where(selector).ToListAsync();
    }
    
    public async ValueTask AddEntities<TEntity>(params TEntity[] entities) where TEntity : class
    {
        var service = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<NoteDbContext>();
        await service.Set<TEntity>().AddRangeAsync(entities);
        await service.SaveChangesAsync();
    }
    
    public async ValueTask DeleteEntities<TEntity>(Expression<Func<TEntity, bool>> selector) where TEntity : class
    {
        var service = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<NoteDbContext>();
        await service.Set<TEntity>().Where(selector).ExecuteDeleteAsync();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}