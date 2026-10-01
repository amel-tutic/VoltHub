using VoltHub.Domain.Entities;

namespace VoltHub.Application.Common.Interfaces;

public sealed record AccessToken(string Token, DateTime ExpiresAt);

public interface ITokenService
{
    AccessToken CreateAccessToken(User user);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
    TimeSpan RefreshTokenLifetime { get; }
}
