using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Authentication;
using NoteAPI.Persistence;
using NoteAPI.ReadModels;
using NoteAPI.Services;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Account;

public record LoginUserRequest(LoginUserRequest.LoginUserRequestBody Body) : IRequest
{
    public record LoginUserRequestBody(string Email, string Password);
}

public class LoginUserEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost<LoginUserRequest, LoginUserRequestHandler>("/api/account/login")
            .Produces<AccessTokenReadModel>()
            .Produces(StatusCodes.Status400BadRequest);
    }
}

public class LoginUserRequestHandler : IRequestHandler<LoginUserRequest>
{
    private readonly NoteDbContext _dbContext;
    private readonly IUserContextService _userContextService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenProvider _jwtTokenProvider;

    public LoginUserRequestHandler(NoteDbContext dbContext,
        IUserContextService userContextService, 
        IPasswordHasher passwordHasher,
        IJwtTokenProvider jwtTokenProvider)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _passwordHasher = passwordHasher;
        _jwtTokenProvider = jwtTokenProvider;
    }
    
    public async ValueTask<IResult> HandleAsync(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.UserId;
        if (userId is not null)
            return Results.BadRequest("You are already logged in.");

        var (email, password) = request.Body;
        
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (user is null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
            return Results.BadRequest("Invalid email or password.");

        var accessToken = _jwtTokenProvider.GenerateJwtToken(user);
        var accessTokenReadModel = new AccessTokenReadModel(accessToken);

        return Results.Ok(accessTokenReadModel);
    }
}