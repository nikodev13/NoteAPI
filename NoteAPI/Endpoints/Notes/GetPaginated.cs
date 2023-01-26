using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Entities;
using NoteAPI.Persistence;
using NoteAPI.ReadModels;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Notes;

public class GetPaginatedNotesEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet<GetPaginatedNotesRequest, GetPaginatedNotesRequestHandler>("/notes");
    }
}

public class GetPaginatedNotesRequest : PaginatedListRequest
{
    public string? SearchPhrase { get; init; }
    public string? OrderBy { get; init; }
    public bool? OrderDescending { get; init; } = false;
}

public class GetPaginatedNotesRequestHandler : IRequestHandler<GetPaginatedNotesRequest>
{
    private readonly NoteDbContext _dbContext;

    public GetPaginatedNotesRequestHandler(NoteDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask<IResult> HandleAsync(GetPaginatedNotesRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Notes.AsNoTracking();

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
            .ToListAsync(cancellationToken);

        var paginatedList = new PaginatedList<NoteReadModel>(notesReadModels, totalCount, request);
        
        return Results.Ok(paginatedList);
    }
}

