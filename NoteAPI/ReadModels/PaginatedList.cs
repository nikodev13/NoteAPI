using Microsoft.EntityFrameworkCore;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.ReadModels;

public class PaginatedListRequest : IRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    
    public PaginatedList(List<T> items, int totalCount , int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
    }

    public static async ValueTask<PaginatedList<T>> CreateFromQueryable(IQueryable<T> queryable, PaginatedListRequest request)
    {
        var totalCount = await queryable.CountAsync();
        var result = await queryable
            .Skip((request.PageNumber-1)*request.PageSize)
            .ToListAsync();

        return new PaginatedList<T>(result, totalCount, request.PageNumber, request.PageSize);
    }
}