using NoteAPI.Endpoints;

namespace NoteAPI.ReadModels;

public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    
    public PaginatedList(List<T> items, int totalCount, PaginatedListRequest request)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = request.PageNumber;
        TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
    }
}