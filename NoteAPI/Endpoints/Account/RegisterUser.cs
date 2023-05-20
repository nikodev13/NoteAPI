using Microsoft.EntityFrameworkCore;
using NoteAPI.Authentication;
using NoteAPI.Entities;
using NoteAPI.Persistence;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Account;

public record RegisterUserRequest(RegisterUserRequest.RegisterUserRequestBody Body) : IRequest
{
    public record RegisterUserRequestBody(string Email, string Password);
}

public class RegisterUserEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost<RegisterUserRequest, RegisterUserRequestHandler>("/api/account/register")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status409Conflict);
    }
}

public class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest>
{
    private readonly NoteDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserRequestHandler(NoteDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }
    
    public async ValueTask<IResult> HandleAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var (email, password) = request.Body;
        
        var userEmailAlreadyExists = await _dbContext.Users
            .AnyAsync(x => x.Email == email, cancellationToken);
        if (userEmailAlreadyExists)
            return Results.Conflict($"User with email `{email}` already exists.");

        var user = new User(email, _passwordHasher.HashPassword(password));

        await _dbContext.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}