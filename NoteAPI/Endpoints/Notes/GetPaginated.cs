using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Entities;
using NoteAPI.Persistence;
using NoteAPI.ReadModels;
using NoteAPI.Services;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Notes;

public record GetPaginatedNotesRequest(
    uint PageSize,
    uint PageNumber,
    string? SearchPhrase = null,
    string? OrderBy = null,
    bool OrderDescending = false) : IRequest;

public class GetPaginatedNotesEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet<GetPaginatedNotesRequest, GetPaginatedNotesRequestHandler>("/api/notes")
            .Produces<PaginatedList<NoteReadModel>>()
            .RequireAuthorization();
    }
}

public class GetPaginatedNotesRequestHandler : IRequestHandler<GetPaginatedNotesRequest>
{
    private readonly NoteDbContext _dbContext;
    private readonly IUserContextService _userContextService;

    public GetPaginatedNotesRequestHandler(NoteDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }
    
    public async ValueTask<IResult> HandleAsync(GetPaginatedNotesRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.UserId;
        var query = _dbContext.Notes.Where(x => x.OwnerId == userId);
        var (pageSize, pageNumber, searchPhrase, orderBy, orderDescending) = request;

        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            query = query.Where(x => ((string)x.Title).Contains(searchPhrase));
        }

        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            Expression<Func<Note, object?>> selector = orderBy switch
            {
                nameof(Note.CreatedAt) => x => x.CreatedAt,
                nameof(Note.LastUpdatedAt) => x => x.LastUpdatedAt,
                _ => x => x.Title
            };
            
            query = orderDescending
                ? query.OrderByDescending(selector)
                : query.OrderBy(selector);
        }

        var totalCount = (uint)await query.CountAsync(cancellationToken);
        
        var notesReadModels = await query
            .Skip((int)((pageNumber-1) * pageSize))
            .Take((int)pageSize)
            .Select(x => NoteReadModel.From(x))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var paginatedList = new PaginatedList<NoteReadModel>(notesReadModels, totalCount, request.PageNumber, request.PageSize);
        
        return Results.Ok(paginatedList);
    }
}

