using System.Net;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using NoteAPI.Authentication;
using NoteAPI.Endpoints.Notes;
using NoteAPI.Entities;
using NoteAPI.ReadModels;
using NoteAPI.Services;
using NoteAPI.ValueObjects;
using Xunit;
using UserContextService = NoteAPI.IntegrationTests.Services.UserContextService;

namespace NoteAPI.IntegrationTests.Tests;

[Collection(TestingCollection.Name)]
public class NotesEndpointsTests : IAsyncLifetime
{
    private readonly Testing _testing;
    private readonly HttpClient _client;

    private readonly Note[] _notes;
    private readonly User _notesOwner;

    public NotesEndpointsTests(Testing testing)
    {
        _testing = testing;
        _client = _testing.GetClient();

        _notesOwner = new User
        {
            UserId = Guid.Parse("48F18513-CF2A-4471-B74B-A0F41E35931B"),
            Email = "notesOwner@test.com",
            PasswordHash = "sample_hash",
            RegisteredAt = DateTime.Now
        };
        _notes = new Note[]
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Sample title 1",
                OwnerId = _notesOwner.UserId,
                CreatedAt = DateTime.Now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Sample title 1",
                OwnerId = _notesOwner.UserId,
                CreatedAt = DateTime.Now
            }
        };
    }
    
    public async Task InitializeAsync()
    {
        await CleanDatabase();
        await _testing.AddEntities(_notesOwner);
        await _testing.AddEntities(_notes);
    }

    public async Task DisposeAsync()
    {
        await CleanDatabase();
    }

    private async Task CleanDatabase()
    {
        await _testing.DeleteEntities<User>(x => x.UserId == _notesOwner.UserId);
    }

    [Theory]
    [InlineData(1, 5, "CCC54951-9185-4A43-85E9-74E59D7EDB7F")]
    [InlineData(1, 10, "61710D4D-D65E-4831-9B62-7C504CC8770C")]
    public async Task GetUserPaginatedNotes_UserHasNoNotes_Returns200OkAndEmptyResult(int pageNumber, int pageSize, string userId)
    {
        // arrange
        UserContextService.CurrentUserId = Guid.Parse(userId);

        // act
        var response = await _client.GetAsync($"/notes?pageNumber={pageNumber}&pageSize={pageSize}");
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
        UserContextService.CurrentUserId = _notesOwner.UserId;

        // act
        var response = await _client.GetAsync("/notes?pageNumber=1&pageSize=5");
        var stringContent = await response.Content.ReadAsStringAsync();
        var content = JsonConvert.DeserializeObject<PaginatedList<NoteReadModel>>(stringContent);

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(content.Items);
    }
    
    [Theory]
    [InlineData("F7E1829A-8F98-4450-9A45-4C00940AF9E2")]
    [InlineData("45981BC2-E1A0-4897-B039-C75CF47E1879")]
    public async Task GetNoteById_UserHasNoNotes_Returns404Notfound(string userId)
    {
        // arrange
        UserContextService.CurrentUserId = Guid.Parse(userId);

        // act
        var response = await _client.GetAsync($"/notes/{_notes[0].Id.Value}");

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetNoteById_UserHasNotes_Returns200OkAndResult()
    {
        // arrange
        UserContextService.CurrentUserId = _notesOwner.UserId;
        var note = _notes[0];
        
        // act
        var response = await _client.GetAsync($"/notes/{note.Id.Value}");
        var content = await response.Content.ReadFromJsonAsync<NoteReadModel>();
        
        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(note.Title, content!.Title);
    }

    [Fact]
    public async Task CreateNote_Successfully_Returns201Created()
    {
        // arrange
        UserContextService.CurrentUserId = _notesOwner.UserId;
        var request = new CreateNoteRequest.CreateNoteRequestBody
        {
            Title = "New title",
            Content = "Any content"
        };
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        // act
        var response = await _client.PostAsync("/notes", httpContent);
        var contentNote = await response.Content.ReadFromJsonAsync<NoteReadModel>();
        
        // assert 
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        // clean
        await _testing.DeleteEntities<Note>(x => x.Id == contentNote!.Id);
    }
    
    [Fact]
    public async Task CreateNote_ThatHasNoUniqueTitle_Returns409Conflict()
    {
        // arrange
        UserContextService.CurrentUserId = _notesOwner.UserId;
        var request = new CreateNoteRequest.CreateNoteRequestBody
        {
            Title = _notes[0].Title,
            Content = "Any content"
        };
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        // act
        var response = await _client.PostAsync("/notes", httpContent);
        
        // assert 
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateNote_ThatHasNoUniqueTitle_Returns409Conflict()
    {
        // arrange
        UserContextService.CurrentUserId = _notesOwner.UserId;
        var request = new UpdateNoteRequest.UpdateNoteRequestBody
        {
            Title = _notes[0].Title,
        };
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        // act
        var response = await _client.PutAsync($"/notes/{_notes[1].Id.Value}", httpContent);
        
        // assert 
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateNote_Successfully_Returns204NoContent()
    {
        // arrange
        UserContextService.CurrentUserId = _notesOwner.UserId;
        var request = new UpdateNoteRequest.UpdateNoteRequestBody
        {
            Title = "New extra unique note title",
        };
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        // act
        var response = await _client.PutAsync($"/notes/{_notes[1].Id.Value}", httpContent);
        
        // assert 
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteNote_Successfully_Returns204NoContent()
    {
        // arrange
        UserContextService.CurrentUserId = _notesOwner.UserId;
        
        // act
        var response = await _client.DeleteAsync($"/notes/{_notes[0].Id.Value}");
        
        // assert 
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteNote_ThatNotBelongTOUser_Returns404NotFound()
    {
        // arrange
        UserContextService.CurrentUserId = Guid.Parse("CCEC53FE-E735-41BF-AA30-D74102E7A18E");
        
        // act
        var response = await _client.DeleteAsync($"/notes/{_notes[0].Id.Value}");
        
        // assert 
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}