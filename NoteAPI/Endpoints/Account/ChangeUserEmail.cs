using Microsoft.EntityFrameworkCore;
using NoteAPI.Authentication;
using NoteAPI.Persistence;
using NoteAPI.Services;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Account;

public record ChangeUserEmailRequest(ChangeUserEmailRequest.ChangeUserEmailRequestBody Body) : IRequest
{
    public record ChangeUserEmailRequestBody(string Email);
}

public class ChangeUserEmailEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost<ChangeUserEmailRequest, ChangeUserEmailRequestHandler>("/api/account/change-email")
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
        var userId = _userContextService.UserId!;
        var email = request.Body.Email;

        var userEmailAlreadyExists = await _noteDbContext.Users
            .AnyAsync(x => x.Email == email, cancellationToken);
        
        if (userEmailAlreadyExists)
            return Results.Conflict($"User with email `{email}` already exists.");

        var user = await _noteDbContext.Users
            .SingleAsync(x => x.UserId.Equals(userId), cancellationToken);

        user.UpdateEmail(email);
        await _noteDbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}