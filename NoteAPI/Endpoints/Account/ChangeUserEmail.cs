using Microsoft.EntityFrameworkCore;
using NoteAPI.Authentication;
using NoteAPI.Persistence;
using NoteAPI.Services;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Account;

public class ChangeUserEmailRequest : IRequest
{
    public required ChangeUserEmailRequestBody Body { get; init; }
    
    public class ChangeUserEmailRequestBody
    {
        public required string Email { get; init; }
    }
}

public class ChangeUserEmailEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost<ChangeUserEmailRequest, ChangeUserEmailRequestHandler>("/account/change-email")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization();
    }
}

public class ChangeUserEmailRequestHandler : IRequestHandler<ChangeUserEmailRequest>
{
    private readonly NoteDbContext _noteDbContext;
    private readonly IUserContextService _userContextService;

    public ChangeUserEmailRequestHandler(NoteDbContext noteDbContext, IUserContextService userContextService)
    {
        _noteDbContext = noteDbContext;
        _userContextService = userContextService;
    }
    
    public async ValueTask<IResult> HandleAsync(ChangeUserEmailRequest request, CancellationToken cancellationToken)
    {
        if (await _noteDbContext.Users.AnyAsync(x => x.Email == request.Body.Email, cancellationToken))
        {
            return Results.Conflict($"User with email `{request.Body.Email}` already exists.");
        }

        var user = await _noteDbContext.Users
            .SingleAsync(x => x.UserId.Value == _userContextService.UserId!.Value, cancellationToken);

        user.Email = request.Body.Email.ToLower();
        await _noteDbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}