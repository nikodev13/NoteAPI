using Microsoft.AspNetCore.Mvc;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Notes;

public class GetAllNotesEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet<GetAllNotesRequest, GetAllNotesRequestHandler>("/notes/{id}");
    }
}

public class GetAllNotesRequest : IRequest
{
    public required int Id { get; init; }
}
public class GetAllNotesRequestHandler : IRequestHandler<GetAllNotesRequest>
{
    public GetAllNotesRequestHandler()
    {
        
    }
    
    public ValueTask<IResult> HandleAsync(GetAllNotesRequest request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(Results.Ok(new { request.Id }));
    }
}

