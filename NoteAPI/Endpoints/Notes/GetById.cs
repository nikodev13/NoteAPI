using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Persistence;
using NoteAPI.ReadModels;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Notes;

public class GetNoteByIdEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet<GetNoteByIdRequest, GetNoteByIdRequestHandler>("/notes/{id:guid}");
    }
}

public class GetNoteByIdRequest : IRequest
{
    [FromRoute]
    public required Guid Id { get; init; }
}

public class GetNoteByIdRequestHandler : IRequestHandler<GetNoteByIdRequest>
{
    private readonly NoteDbContext _dbContext;

    public GetNoteByIdRequestHandler(NoteDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask<IResult> HandleAsync(GetNoteByIdRequest request, CancellationToken cancellationToken)
    {
        var noteReadModel = await _dbContext.Notes
            .Where(x => x.Id == request.Id)
            .Select(x => NoteReadModel.From(x))
            .FirstOrDefaultAsync(cancellationToken);

        return noteReadModel is null
            ? Results.NotFound($"Note with id {request.Id} not found.")
            : Results.Ok(noteReadModel);
    }
}