using NoteAPI.ValueObjects;

namespace NoteAPI.Entities;

public class User
{
    public required UserId UserId { get; init; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime RegisteredAt { get; set; }
}