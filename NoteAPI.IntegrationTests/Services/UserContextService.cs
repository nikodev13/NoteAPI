using NoteAPI.Services;
using NoteAPI.ValueObjects;

namespace NoteAPI.IntegrationTests.Services;

public class UserContextService : IUserContextService
{
    public UserId? UserId => CurrentUserId;
    public static UserId? CurrentUserId { get; set; }
}