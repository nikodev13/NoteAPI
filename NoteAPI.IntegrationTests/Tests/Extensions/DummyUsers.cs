using NoteAPI.Authentication;
using NoteAPI.Entities;

namespace NoteAPI.IntegrationTests.Tests.Extensions;

public static class DummyUsers
{
    public static List<User> Users { get; }
    
    static DummyUsers()
    {
        var passwordHasher = new BCryptPasswordHasher();
        
        Users = new List<User>()
        {
            new()
            {
                UserId = Guid.Parse("48F18513-CF2A-4471-B74B-A0F41E35931B"),
                Email = "notesOwner@test.com",
                PasswordHash = passwordHasher.HashPassword("sample_password"),
                RegisteredAt = DateTime.Now,
            }
        };
    }
}