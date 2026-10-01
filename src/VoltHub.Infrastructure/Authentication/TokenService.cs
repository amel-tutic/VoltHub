using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Domain.Entities;

namespace VoltHub.Infrastructure.Authentication;

internal sealed class TokenService(IOptions<JwtSettings> options) : ITokenService
{
    private readonly JwtSettings _settings = options.Value;
    private readonly JsonWebTokenHandler _handler = new();

    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(_settings.RefreshTokenDays);

    public AccessToken CreateAccessToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes);
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            Expires = expiresAt,
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),   // who
                [JwtRegisteredClaimNames.Email] = user.Email,
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
                ["role"] = user.Role.ToString()                        // what they may do
            },
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SigningKey)),
                SecurityAlgorithms.HmacSha256)
        };

        return new AccessToken(_handler.CreateToken(descriptor), expiresAt);
    }

    // 64 random bytes: unguessable, and URL-safe so clients can send it anywhere.
    public string GenerateRefreshToken() => Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));

    public string HashRefreshToken(string refreshToken) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
}
