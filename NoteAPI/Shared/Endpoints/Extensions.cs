using Microsoft.AspNetCore.Mvc;

namespace NoteAPI.Shared.Endpoints;

public static class Extensions
{
    public static RouteHandlerBuilder MapGet<TQuery, TQueryHandler>(this IEndpointRouteBuilder endpointBuilder, string template)
        where TQuery : IRequest
        where TQueryHandler : IRequestHandler<TQuery>
    {
        return endpointBuilder.MapGet(template,
            async ([FromServices] TQueryHandler endpointHandler,
                    [AsParameters] TQuery request,
                    CancellationToken cancellationToken)
                => await endpointHandler.HandleAsync(request, cancellationToken)
        );
    }
    
    public static RouteHandlerBuilder MapPost<TRequest, TRequestHandler>(this IEndpointRouteBuilder endpointBuilder, string template)
        where TRequest : IRequest
        where TRequestHandler : IRequestHandler<TRequest>
    {
        return endpointBuilder.MapPost(template,
            async ([FromServices] TRequestHandler endpointHandler,
                    [AsParameters] TRequest request,
                    CancellationToken cancellationToken)
                => await endpointHandler.HandleAsync(request, cancellationToken)
        );
    }
 
    public static RouteHandlerBuilder MapPut<TRequest, TRequestHandler>(this IEndpointRouteBuilder endpointBuilder, string template)
        where TRequest : IRequest
        where TRequestHandler : IRequestHandler<TRequest>
    {
        return endpointBuilder.MapPut(template,
            async ([FromServices] TRequestHandler endpointHandler,
                    [AsParameters] TRequest request,
                    CancellationToken cancellationToken)
                => await endpointHandler.HandleAsync(request, cancellationToken)
        );
    }

    
    public static RouteHandlerBuilder MapDelete<TRequest, TRequestHandler>(this IEndpointRouteBuilder endpointBuilder, string template)
        where TRequest : IRequest
        where TRequestHandler : IRequestHandler<TRequest>
    {
        return endpointBuilder.MapDelete(template,
            async ([FromServices] TRequestHandler endpointHandler,
                    [AsParameters] TRequest request,
                    CancellationToken cancellationToken)
                => await endpointHandler.HandleAsync(request, cancellationToken)
        );
    }

 
}