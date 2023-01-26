using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Persistence;
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
        endpoint.MapDelete<DeleteNoteRequest, DeleteNoteRequestHandler>("/notes/{id:guid}");
    }
}

public class DeleteNoteRequestHandler : IRequestHandler<DeleteNoteRequest>
{
    private readonly NoteDbContext _dbContext;

    public DeleteNoteRequestHandler(NoteDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask<IResult> HandleAsync(DeleteNoteRequest request, CancellationToken cancellationToken)
    {
        var affectedRows = await _dbContext.Notes
            .Where(x => x.Id == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        return affectedRows == 0
            ? Results.NotFound($"Note with id {request.Id} not found.")
            : Results.NoContent();
    }
}