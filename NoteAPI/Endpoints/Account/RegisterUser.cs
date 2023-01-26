using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Authentication;
using NoteAPI.Entities;
using NoteAPI.Persistence;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Account;

public class RegisterUserRequest : IRequest
{
    [FromBody] 
    public required RegisterUserRequestBody Body { get; init; }
    
    public class RegisterUserRequestBody
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    } 
}

public class RegisterUserEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost<RegisterUserRequest, RegisterUserRequestHandler>("/account/register")
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
        if (await _dbContext.Users.AnyAsync(x => x.Email == request.Body.Email, cancellationToken))
        {
            return Results.Conflict($"User with email `{request.Body.Email}` already exists.");
        }

        var user = new User()
        {
            UserId = Guid.NewGuid(),
            Email = request.Body.Email.ToLower(),
            PasswordHash = _passwordHasher.HashPassword(request.Body.Password),
            RegisteredAt = DateTime.Now
        };

        await _dbContext.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}