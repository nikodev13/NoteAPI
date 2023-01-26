using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Persistence;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Notes;

public class UpdateNoteRequest : IRequest
{
    [FromRoute]
    public required Guid Id { get; init; }
    [FromBody]
    public required UpdateNoteRequestBody Body { get; init; }
    
    public class UpdateNoteRequestBody
    {
        public required string Title { get; init; }
        public string? Content { get; init; }
    }
}

public class UpdateNoteEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPut<UpdateNoteRequest, UpdateNoteRequestHandler>("/notes/{id:guid}");
    }
}

public class UpdateNoteRequestHandler : IRequestHandler<UpdateNoteRequest>
{
    private readonly NoteDbContext _dbContext;

    public UpdateNoteRequestHandler(NoteDbContext _dbContext)
    {
        this._dbContext = _dbContext;
    }
    
    public async ValueTask<IResult> HandleAsync(UpdateNoteRequest request, CancellationToken cancellationToken)
    {
        if (await _dbContext.Notes.AnyAsync(x => x.Title == request.Body.Title, cancellationToken))
        {
            return Results.BadRequest($"Note with title `{request.Body.Title}` already exists.");
        }

        var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (note is null)
        {
            return Results.NotFound($"Note with id {request.Id} not found.");
        }

        note.Title = request.Body.Title;
        note.Content = request.Body.Content;
        note.LastUpdatedAt = DateTime.Now;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Results.Accepted();
    }
}