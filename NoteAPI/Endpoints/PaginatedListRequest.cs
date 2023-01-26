using Microsoft.AspNetCore.Mvc;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints;

public class PaginatedListRequest : IRequest
{
    [FromQuery]
    public int PageNumber { get; init; } = 1;
    [FromQuery]
    public int PageSize { get; init; } = 10;
}

