using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using NoteAPI.ValueObjects;

namespace NoteAPI.Services;

public interface IUserContextService
{
    public UserId? UserId { get; }
}

public class UserContextService : IUserContextService
{
    public UserId? UserId { get; }

    public UserContextService(IHttpContextAccessor contextAccessor)
    {
        var claim = contextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        UserId = Guid.TryParse(claim, out var parsedGuid)
            ? new UserId(parsedGuid)
            : null;
    }
}