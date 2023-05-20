using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Persistence;
using NoteAPI.ReadModels;
using NoteAPI.Services;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Notes;

public record GetNoteByIdRequest(Guid Id) : IRequest;

public class GetNoteByIdEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet<GetNoteByIdRequest, GetNoteByIdRequestHandler>("/api/notes/{id:guid}")
            .Produces<NoteReadModel>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }
}

public class GetNoteByIdRequestHandler : IRequestHandler<GetNoteByIdRequest>
{
    private readonly NoteDbContext _dbContext;
    private readonly IUserContextService _userContextService;

    public GetNoteByIdRequestHandler(NoteDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }
    
    public async ValueTask<IResult> HandleAsync(GetNoteByIdRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.UserId;
        var noteId = request.Id;
        
        var noteReadModel = await _dbContext.Notes
            .Where(x => x.OwnerId.Equals(userId) && x.Id.Equals(noteId))
            .Select(x => NoteReadModel.From(x))
            .FirstOrDefaultAsync(cancellationToken);

        return noteReadModel is null
            ? Results.NotFound($"Note with id {noteId} not found.")
            : Results.Ok(noteReadModel);
    }
}