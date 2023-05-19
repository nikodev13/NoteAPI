using System.Net;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using NoteAPI.Endpoints.Notes;
using NoteAPI.IntegrationTests.Tests.Extensions;
using NoteAPI.ReadModels;
using Xunit;
using UserContextService = NoteAPI.IntegrationTests.Services.UserContextService;

namespace NoteAPI.IntegrationTests.Tests;

[Collection(TestingCollection.Name)]
public class NotesEndpointsTests
{
    private readonly Testing _testing;
    private readonly HttpClient _client;
    
    public NotesEndpointsTests(Testing testing)
    {
        _testing = testing;
        _client = testing.Client;
    }

    [Theory]
    [InlineData(1, 5, "CCC54951-9185-4A43-85E9-74E59D7EDB7F")]
    [InlineData(1, 10, "61710D4D-D65E-4831-9B62-7C504CC8770C")]
    public async Task GetUserPaginatedNotes_UserHasNoNotes_Returns200OkAndEmptyResult(int pageNumber, int pageSize, string userId)
    {
        // arrange
        UserContextService.CurrentUserId = Guid.Parse(userId);

        // act
        var response = await _client.GetAsync($"/api/notes?pageNumber={pageNumber}&pageSize={pageSize}");
        var stringContent = await response.Content.ReadAsStringAsync();
        var content = JsonConvert.DeserializeObject<PaginatedList<NoteReadModel>>(stringContent);
        
        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(content!.Items);
    }

    [Fact]
    public async Task GetUserPaginatedNotes_UserHasNotes_Returns200OkAndNOTEmptyResult()
    {
        // arrange
        var userId = DummyUsers.Users[0].UserId;
        UserContextService.CurrentUserId = userId;
        await _testing.AddUniqueNoteToDb(userId);
        
        // act
        var response = await _client.GetAsync("/api/notes?pageNumber=1&pageSize=5");
        var stringContent = await response.Content.ReadAsStringAsync();
        var content = JsonConvert.DeserializeObject<PaginatedList<NoteReadModel>>(stringContent);

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(content!.Items);
    }
    
    [Theory]
    [InlineData("F7E1829A-8F98-4450-9A45-4C00940AF9E2")]
    [InlineData("45981BC2-E1A0-4897-B039-C75CF47E1879")]
    public async Task GetNoteById_UserHasNoNotes_Returns404Notfound(string userId)
    {
        // arrange
        var note = await _testing.AddUniqueNoteToDb(DummyUsers.Users[0].UserId);
        UserContextService.CurrentUserId = Guid.Parse(userId);

        // act
        var response = await _client.GetAsync($"/api/notes/{note.Id.Value}");

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetNoteById_UserHasNotes_Returns200OkAndResult()
    {
        // arrange
        var userId = DummyUsers.Users[0].UserId;
        UserContextService.CurrentUserId = userId;
        var note = await _testing.AddUniqueNoteToDb(userId);
        
        // act
        var response = await _client.GetAsync($"/api/notes/{note.Id.Value}");
        var content = await response.Content.ReadFromJsonAsync<NoteReadModel>();
        
        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(note.Title, content!.Title);
    }

    [Fact]
    public async Task CreateNote_Successfully_Returns201Created()
    {
        // arrange
        UserContextService.CurrentUserId = DummyUsers.Users[0].UserId;
        var request = new CreateNoteRequest.CreateNoteRequestBody
        {
            Title = "New title",
            Content = "Any content"
        };
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        // act
        var response = await _client.PostAsync("/api/notes", httpContent);
        //var contentNote = await response.Content.ReadFromJsonAsync<NoteReadModel>();
        
        // assert 
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateNote_ThatHasNoUniqueTitle_Returns409Conflict()
    {
        // arrange
        var userId = DummyUsers.Users[0].UserId;
        UserContextService.CurrentUserId = userId;
        var note = await _testing.AddUniqueNoteToDb(userId);
        var request = new CreateNoteRequest.CreateNoteRequestBody
        {
            Title = note.Title,
            Content = "Any content"
        };
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        // act
        var response = await _client.PostAsync("/api/notes", httpContent);
        
        // assert 
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateNote_ThatHasNoUniqueTitle_Returns409Conflict()
    {
        // arrange
        var userId = DummyUsers.Users[0].UserId;
        UserContextService.CurrentUserId = userId;
        var note1 = await _testing.AddUniqueNoteToDb(userId);
        var note2 = await _testing.AddUniqueNoteToDb(userId);
        var request = new UpdateNoteRequest.UpdateNoteRequestBody
        {
            Title = note1.Title,
        };
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        // act
        var response = await _client.PutAsync($"/api/notes/{note2.Id.Value}", httpContent);
        
        // assert 
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateNote_Successfully_Returns204NoContent()
    {
        // arrange
        var userId = DummyUsers.Users[0].UserId;
        UserContextService.CurrentUserId = userId;
        var note = await _testing.AddUniqueNoteToDb(userId);
        var request = new UpdateNoteRequest.UpdateNoteRequestBody
        {
            Title = "New extra unique note title",
        };
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        // act
        var response = await _client.PutAsync($"/api/notes/{note.Id.Value}", httpContent);
        
        // assert 
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteNote_Successfully_Returns204NoContent()
    {
        // arrange
        var userId = DummyUsers.Users[0].UserId;
        UserContextService.CurrentUserId = userId;
        var note = await _testing.AddUniqueNoteToDb(userId);
        
        // act
        var response = await _client.DeleteAsync($"/api/notes/{note.Id.Value}");
        
        // assert 
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteNote_ThatNotBelongTOUser_Returns404NotFound()
    {
        // arrange
        var note = await _testing.AddUniqueNoteToDb(DummyUsers.Users[0].UserId);
        UserContextService.CurrentUserId = Guid.NewGuid();

        // act
        var response = await _client.DeleteAsync($"/api/notes/{note.Id.Value}");
        
        // assert 
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}