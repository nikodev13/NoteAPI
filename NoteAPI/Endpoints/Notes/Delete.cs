using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Persistence;
using NoteAPI.Services;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Notes;

public class DeleteNoteRequest : IRequest
{
    [FromRoute]
    public required Guid Id { get; init; }
}

public class DeleteNoteEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapDelete<DeleteNoteRequest, DeleteNoteRequestHandler>("/api/notes/{id:guid}")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }
}

public class DeleteNoteRequestHandler : IRequestHandler<DeleteNoteRequest>
{
    private readonly NoteDbContext _dbContext;
    private readonly IUserContextService _userContextService;

    public DeleteNoteRequestHandler(NoteDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }
    
    public async ValueTask<IResult> HandleAsync(DeleteNoteRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.UserId;
        
        var affectedRows = await _dbContext.Notes
            .Where(x => x.OwnerId == userId && x.Id == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        return affectedRows == 0
            ? Results.NotFound($"Note with id {request.Id} not found.")
            : Results.NoContent();
    }
}