using Microsoft.AspNetCore.Mvc;

namespace NoteAPI.Shared.Endpoints;

public static class Extensions
{
    public static RouteHandlerBuilder MapGet<TQuery, TQueryHandler>(this IEndpointRouteBuilder endpointBuilder, string template)
        where TQuery : IQuery
        where TQueryHandler : IQueryHandler<TQuery>
    {
        return endpointBuilder.MapGet(template,
            async ([FromServices] TQueryHandler endpointHandler,
                    [AsParameters] TQuery request,
                    CancellationToken cancellationToken)
                => await endpointHandler.HandleAsync(request, cancellationToken)
        );
    }
    
    public static RouteHandlerBuilder MapPost<TCommand, TCommandHandler>(this IEndpointRouteBuilder endpointBuilder, string template)
        where TCommand : ICommand
        where TCommandHandler : ICommandHandler<TCommand>
    {
        return endpointBuilder.MapGet(template,
            async ([FromServices] TCommandHandler endpointHandler,
                    [AsParameters] TCommand request,
                    CancellationToken cancellationToken)
                => await endpointHandler.HandleAsync(request, cancellationToken)
        );
    }
    
    public static RouteHandlerBuilder MapPut<TCommand, TCommandHandler>(this IEndpointRouteBuilder endpointBuilder, string template)
        where TCommand : ICommand
        where TCommandHandler : ICommandHandler<TCommand>
    {
        return endpointBuilder.MapGet(template,
            async ([FromServices] TCommandHandler endpointHandler,
                    [AsParameters] TCommand request,
                    CancellationToken cancellationToken)
                => await endpointHandler.HandleAsync(request, cancellationToken)
        );
    }
    
    public static RouteHandlerBuilder MapDelete<TCommand, TCommandHandler>(this IEndpointRouteBuilder endpointBuilder, string template)
        where TCommand : ICommand
        where TCommandHandler : ICommandHandler<TCommand>
    {
        return endpointBuilder.MapGet(template,
            async ([FromServices] TCommandHandler endpointHandler,
                    [AsParameters] TCommand request,
                    CancellationToken cancellationToken)
                => await endpointHandler.HandleAsync(request, cancellationToken)
        );
    }
}