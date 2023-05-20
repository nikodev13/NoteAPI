using NoteAPI.ValueObjects;

namespace NoteAPI.Entities;

public class User
{
    public UserId UserId { get; }
    public string Email { get; private set; }
    public string PasswordHash { get; set; }
    public DateTime RegisteredAt { get; }

    public User(string email, string passwordHash)
    {
        UserId = Guid.NewGuid();
        Email = email.ToLower();
        PasswordHash = passwordHash;
        RegisteredAt = DateTime.Now;
    }

    public void UpdateEmail(string email)
    {
        Email = email.ToLower();
    }
}