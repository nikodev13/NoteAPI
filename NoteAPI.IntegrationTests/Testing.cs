using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NoteAPI.Entities;
using NoteAPI.IntegrationTests.Helpers;
using NoteAPI.IntegrationTests.Tests.Extensions;
using NoteAPI.Persistence;
using NoteAPI.Shared.Endpoints;
using Xunit;

namespace NoteAPI.IntegrationTests;

[CollectionDefinition(TestingCollection.Name)]
public class TestingCollection : ICollectionFixture<Testing>
{
    public const string Name = "Testing";
}

public class Testing : IDisposable, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly IServiceScopeFactory _scopeFactory;
    public HttpClient Client { get; }

    public Testing()
    {
        _factory = new CustomWebApplicationFactory();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        Client = _factory.CreateClient();
    }

    public async ValueTask<List<TEntity>> FindEntities<TEntity>(Expression<Func<TEntity, bool>> selector) where TEntity : class
    {
        var dbContext = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<NoteDbContext>();
        return await dbContext.Set<TEntity>().Where(selector).ToListAsync();
    }
    
    public async ValueTask AddEntities<TEntity>(params TEntity[] entities) where TEntity : class
    {
        var dbContext = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<NoteDbContext>();
        await dbContext.Set<TEntity>().AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }
    
    public async ValueTask DeleteEntities<TEntity>(Expression<Func<TEntity, bool>> selector) where TEntity : class
    {
        var dbContext = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<NoteDbContext>();
        await dbContext.Set<TEntity>().Where(selector).ExecuteDeleteAsync();
    }

    public void Dispose()
    {
        Client.Dispose();
        _factory.Dispose();
    }

    public async Task InitializeAsync()
    {
        await ClearDatabase();
        await AddEntities(DummyUsers.Users.ToArray());
    }

    public async Task DisposeAsync()
    {
        await ClearDatabase();
    }

    private async Task ClearDatabase()
    {
        var usersIds = DummyUsers.Users.Select(x => x.UserId).ToList();
        await DeleteEntities((User x) => usersIds.Contains(x.UserId));
        // await NoteDbManager.DeleteAll();
    }
}