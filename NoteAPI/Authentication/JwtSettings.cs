using NoteAPI.Shared.Settings;

namespace NoteAPI.Authentication;

public class JwtSettings : ISettings
{
    public string SecretKey { get; init; } = default!;
    public uint AccessTokenExpireDays { get; init; } = default!;
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;

    public static string SectionName => nameof(JwtSettings);
}
