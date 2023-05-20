using System.Net;
using System.Text;
using Newtonsoft.Json;
using NoteAPI.Endpoints.Account;
using NoteAPI.IntegrationTests.Services;
using NoteAPI.IntegrationTests.Tests.Extensions;
using NoteAPI.ReadModels;
using Xunit;

namespace NoteAPI.IntegrationTests.Tests;

[Collection(TestingCollection.Name)]
public class UserEndpointsTests
{
    private readonly HttpClient _client;

    public UserEndpointsTests(Testing testing)
    {
        _client = testing.Client;
    }
    
    [Fact]
    public async Task RegisterUser_WithEmailThatAlreadyExists_Returns409Conflict()
    {
        // arrange
        var request = new RegisterUserRequest.RegisterUserRequestBody(DummyUsers.Users[0].Email, "sample_password");
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        //act
        var result = await _client.PostAsync("/api/account/register", httpContent);
        
        // assert
        Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
    }
    
    [Fact]
    public async Task LoginUser_InvalidEmailOrPassword_Returns400BadRequest()
    {
        // arrange
        var request = new LoginUserRequest.LoginUserRequestBody(DummyUsers.Users[0].Email, "sample_invalid_password");
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        //act
        var result = await _client.PostAsync("/api/account/login", httpContent);
        
        // assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task LoginUser_Successfully_Returns200OkAndAccessTokenReadModel()
    {
        // arrange
        var request = new LoginUserRequest.LoginUserRequestBody(DummyUsers.Users[0].Email, "sample_password");
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        //act
        var result = await _client.PostAsync("/api/account/login", httpContent);
        var responseBody = JsonConvert.DeserializeObject<AccessTokenReadModel>(await result.Content.ReadAsStringAsync());
        
        // assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotEmpty(responseBody!.AccessToken);
    }
    
    [Fact]
    public async Task ChangeEmail_WithEmailThatAlreadyExists_Returns409Conflict()
    {
        // arrange
        var request = new ChangeUserEmailRequest.ChangeUserEmailRequestBody(DummyUsers.Users[0].Email);
        
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        //act
        var result = await _client.PostAsync("/api/account/change-email", httpContent);
        
        // assert
        Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
    }
    
    [Fact]
    public async Task ChangeEmail_Successfully_ReturnsNoContent()
    {
        // arrange
        var request = new ChangeUserEmailRequest.ChangeUserEmailRequestBody("newtest@test.com");

        UserContextService.CurrentUserId = DummyUsers.Users[0].UserId;
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        //act
        var result = await _client.PostAsync("/api/account/change-email", httpContent);
        //var content = await result.Content.ReadAsStringAsync();
        // assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }
}