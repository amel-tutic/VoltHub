namespace VoltHub.Infrastructure.Authentication;

// Bound from the "Jwt" section of configuration. SigningKey comes from user-secrets, never from appsettings.
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string SigningKey { get; init; } = default!;
    public int AccessTokenMinutes { get; init; } = 15;
    public int RefreshTokenDays { get; init; } = 7;
}
