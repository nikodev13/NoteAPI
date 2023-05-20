using Microsoft.AspNetCore.Mvc;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints;

public record PaginatedListRequest(uint PageNumber, uint PageSize) : IRequest;
