namespace NoteAPI.Authentication;

public interface IPasswordHasher
{ 
    string HashPassword(string passwordToHash);
    bool VerifyPassword(string passwordToVerify, string passwordHash);
}

public class BCryptPasswordHasher : IPasswordHasher
{
    private string GenerateSalt() => BCrypt.Net.BCrypt.GenerateSalt(9); 
    
    public string HashPassword(string passwordToHash)
    {
        return BCrypt.Net.BCrypt.HashPassword(passwordToHash, GenerateSalt());
    }

    public bool VerifyPassword(string passwordToVerify, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(passwordToVerify, passwordHash);
    }
}