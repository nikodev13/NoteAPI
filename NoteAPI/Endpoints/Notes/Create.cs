using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Entities;
using NoteAPI.Persistence;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Notes;

public class CreateNoteRequest : IRequest
{
    [FromBody]
    public CreateNoteRequestBody Body { get; init; }
    
    public class CreateNoteRequestBody
    {
        public required string Title { get; init; }
        public string? Content { get; init; }
    }
}

public class CreateNoteEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost<CreateNoteRequest, CreateNoteRequestHandler>("/notes");
    }
}

public class CreateNoteRequestHandler : IRequestHandler<CreateNoteRequest>
{
    private readonly NoteDbContext _dbContext;

    public CreateNoteRequestHandler(NoteDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask<IResult> HandleAsync(CreateNoteRequest request, CancellationToken cancellationToken)
    {
        if (await _dbContext.Notes.AnyAsync(x => x.Title == request.Body.Title, cancellationToken))
        {
            return Results.BadRequest($"Note with title `{request.Body.Title}` already exists.");
        }

        var note = new Note()
        {
            Id = Guid.NewGuid(),
            Title = request.Body.Title,
            Content = request.Body.Content,
            OwnerId = Guid.Parse("7DB84255-D006-42F0-A517-0A4A5F7FACB8"),
            CreatedAt = DateTime.Now
        };

        await _dbContext.AddAsync(note, cancellationToken);

        return Results.Created($"/notes/{note.Id}", note);
    }
}