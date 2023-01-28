using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using NoteAPI.Persistence;
using NoteAPI.Services;
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
        endpoint.MapPut<UpdateNoteRequest, UpdateNoteRequestHandler>("/api/notes/{id:guid}")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization();
    }
}

public class UpdateNoteRequestHandler : IRequestHandler<UpdateNoteRequest>
{
    private readonly NoteDbContext _dbContext;
    private readonly IUserContextService _userContextService;

    public UpdateNoteRequestHandler(NoteDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }
    
    public async ValueTask<IResult> HandleAsync(UpdateNoteRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.UserId;
        
        if (await _dbContext.Notes.AnyAsync(x => x.OwnerId == userId && x.Title == request.Body.Title && x.Id != request.Id, cancellationToken))
        {
            return Results.Conflict($"Note with title `{request.Body.Title}` already exists.");
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

        return Results.NoContent();
    }
}