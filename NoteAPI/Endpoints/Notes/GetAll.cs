using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Notes;

public class GetAllNotesEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet<GetAllNotesQuery, GetAllNotesQueryHandler>("/notes");
    }
}

public class GetAllNotesQuery : IQuery {}
public class GetAllNotesQueryHandler : IQueryHandler<GetAllNotesQuery>
{
    public GetAllNotesQueryHandler()
    {
        
    }
    
    public ValueTask<IResult> HandleAsync(GetAllNotesQuery query, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(Results.Ok(1));
    }
}

