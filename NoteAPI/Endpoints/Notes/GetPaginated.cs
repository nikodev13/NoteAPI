using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Entities;
using NoteAPI.Persistence;
using NoteAPI.ReadModels;
using NoteAPI.Services;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Notes;

public class GetPaginatedNotesRequest : PaginatedListRequest
{
    [FromQuery]
    public string? SearchPhrase { get; init; }
    [FromQuery]
    public string? OrderBy { get; init; }
    [FromQuery]
    public bool? OrderDescending { get; init; } = false;
}

public class GetPaginatedNotesEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet<GetPaginatedNotesRequest, GetPaginatedNotesRequestHandler>("/notes")
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

        if (!string.IsNullOrWhiteSpace(request.SearchPhrase))
        {
            query = query.Where(x => x.Title.Contains(request.SearchPhrase));
        }

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            var orderSelectors = new Dictionary<string, Expression<Func<Note, object>>>()
            {
                { "Title", x => x.Title },
                { "CreatedAt", x => x.CreatedAt },
                { "LastModifiedAt", x => x.LastUpdatedAt! },
            };

            var selector = orderSelectors[request.OrderBy];
            
            query = request.OrderDescending == true
                ? query.OrderByDescending(selector)
                : query.OrderBy(selector);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        var notesReadModels = await query
            .Skip((request.PageNumber-1)*request.PageSize)
            .Take(request.PageSize)
            .Select(x => NoteReadModel.From(x))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var paginatedList = new PaginatedList<NoteReadModel>(notesReadModels, totalCount, request.PageNumber, request.PageSize);
        
        return Results.Ok(paginatedList);
    }
}

