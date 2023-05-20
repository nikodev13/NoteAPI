using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using NoteAPI.Persistence;
using NoteAPI.Services;
using NoteAPI.Shared.Endpoints;
using NoteAPI.ValueObjects;

namespace NoteAPI.Endpoints.Notes;

public record UpdateNoteRequest(Guid Id, UpdateNoteRequest.UpdateNoteRequestBody Body) : IRequest
{
    public record UpdateNoteRequestBody(string Title, string? Content = null);
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
        var (noteId, (title, content)) = request;
        
        var noteAlreadyExists = await _dbContext.Notes
            .Where(x => x.OwnerId.Equals(userId) && x.Title == title)
            .AnyAsync(cancellationToken);
        if (noteAlreadyExists)
            return Results.Conflict($"Note with title `{title}` already exists.");

        var note = await _dbContext.Notes
            .FirstOrDefaultAsync(x => x.Id.Equals(noteId), cancellationToken);
        if (note is null)
            return Results.NotFound($"Note with id {noteId} not found.");

        note.UpdateTitle(title);
        note.UpdateContent(content);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}