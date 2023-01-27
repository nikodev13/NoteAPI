using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NoteAPI.Authentication;
using NoteAPI.Endpoints.Account;
using NoteAPI.Entities;
using NoteAPI.IntegrationTests.Helpers;
using NoteAPI.IntegrationTests.Services;
using NoteAPI.ReadModels;
using Xunit;

namespace NoteAPI.IntegrationTests.Tests;

[Collection(TestingCollection.Name)]
public class UserEndpointsTests : IAsyncLifetime
{
    private readonly Testing _testing;
    private readonly HttpClient _client;

    private User[] _users;

    public UserEndpointsTests(Testing testing)
    {
        _testing = testing;
        _client = testing.GetClient();

        _users = new User[]
        {
            new()
            {
                UserId = Guid.Parse("EAE51B85-1749-4BF6-B836-B3F4A63A5DA5"),
                Email = "test@test.com",
                PasswordHash = new BCryptPasswordHasher().HashPassword("sample_password"),
                RegisteredAt = DateTime.Now
            }
        };
    }
    
    public async Task InitializeAsync()
    {
        await CleanDatabase();
        await _testing.AddEntities(_users);
    }

    public async Task DisposeAsync()
    {
        // await CleanDatabase();
    }

    private async Task CleanDatabase()
    {
        var ids = _users.Select(x => x.UserId.Value).ToArray();
        await _testing.DeleteEntities<User>(
            x => ids.Contains(x.UserId));
    }

    [Fact]
    public async Task RegisterUser_WithEmailThatAlreadyExists_Returns409Conflict()
    {
        // arrange
        var request = new RegisterUserRequest.RegisterUserRequestBody
        {
            Email = _users[0].Email,
            Password = "sample_password"
        };
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        //act
        var result = await _client.PostAsync("/account/register", httpContent);
        
        // assert
        Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
    }
    
    [Fact]
    public async Task LoginUser_InvalidEmailOrPassword_Returns400BadRequest()
    {
        // arrange
        var request = new LoginUserRequest.LoginUserRequestBody
        {
            Email = _users[0].Email,
            Password = "sample_invalid_password"
        };
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        //act
        var result = await _client.PostAsync("/account/login", httpContent);
        
        // assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task LoginUser_Successfully_Returns200OkAndAccessTokenReadModel()
    {
        // arrange
        var request = new LoginUserRequest.LoginUserRequestBody
        {
            Email = _users[0].Email,
            Password = "sample_password"
        };
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        //act
        var result = await _client.PostAsync("/account/login", httpContent);
        var responseBody = JsonConvert.DeserializeObject<AccessTokenReadModel>(await result.Content.ReadAsStringAsync());
        
        // assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotEmpty(responseBody.AccessToken);
    }
    
    [Fact]
    public async Task ChangeEmail_WithEmailThatAlreadyExists_Returns409Conflict()
    {
        // arrange
        var request = new ChangeUserEmailRequest.ChangeUserEmailRequestBody()
        {
            Email = _users[0].Email,
        };
        
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        //act
        var result = await _client.PostAsync("/account/change-email", httpContent);
        
        // assert
        Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
    }
    
    [Fact]
    public async Task ChangeEmail_Successfully_ReturnsNoContent()
    {
        // arrange
        var request = new ChangeUserEmailRequest.ChangeUserEmailRequestBody()
        {
            Email = "newtest@test.com",
        };
        UserContextService.CurrentUserId = _users[0].UserId;
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        //act
        var result = await _client.PostAsync("/account/change-email", httpContent);
        var content = await result.Content.ReadAsStringAsync();
        // assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }
    
    
}