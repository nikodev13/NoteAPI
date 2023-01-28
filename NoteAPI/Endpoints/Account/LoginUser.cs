using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Authentication;
using NoteAPI.Persistence;
using NoteAPI.ReadModels;
using NoteAPI.Services;
using NoteAPI.Shared.Endpoints;

namespace NoteAPI.Endpoints.Account;

public class LoginUserRequest : IRequest
{
    [FromBody]
    public required LoginUserRequestBody Body { get; set; }
    
    public class LoginUserRequestBody
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
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
        if (_userContextService.UserId is not null)
        {
            return Results.BadRequest("You're already logged in.");
        }
        
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Body.Email, cancellationToken);
        if (user is null || !_passwordHasher.VerifyPassword(request.Body.Password, user.PasswordHash))
        {
            return Results.BadRequest("Invalid email or password.");
        }

        var accessTokenReadModel = new AccessTokenReadModel
        {
            AccessToken = _jwtTokenProvider.GenerateJwtToken(user)
        };

        return Results.Ok(accessTokenReadModel);
    }
}