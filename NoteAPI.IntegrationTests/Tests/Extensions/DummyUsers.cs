using NoteAPI.Authentication;
using NoteAPI.Entities;

namespace NoteAPI.IntegrationTests.Tests.Extensions;

public static class DummyUsers
{
    public static List<User> Users { get; }
    
    static DummyUsers()
    {
        var passwordHasher = new BCryptPasswordHasher();
        
        Users = new List<User>
        {
            new("notesOwner@test.com", passwordHasher.HashPassword("sample_password"))
        };
    }
}