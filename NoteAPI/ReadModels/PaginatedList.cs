using NoteAPI.Endpoints;

namespace NoteAPI.ReadModels;

public class PaginatedList<T>
{
    public List<T> Items { get; }
    public uint PageNumber { get; }
    public uint TotalPages { get; }
    public uint TotalCount { get; }
    
    public PaginatedList(List<T> items, uint totalCount, uint pageNumber, uint pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        TotalPages = (uint)Math.Ceiling((double)totalCount / pageSize);
    }
}