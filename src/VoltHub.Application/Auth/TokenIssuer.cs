using VoltHub.Application.Common.Interfaces;
using VoltHub.Domain.Entities;

namespace VoltHub.Application.Auth;

// Shared by register, login and refresh: a short-lived access token plus a new refresh token.
// Only the refresh token's hash is stored; the client is the only holder of the real value.
internal static class TokenIssuer
{
    public static AuthResponse Issue(User user, ITokenService tokens, IApplicationDbContext db)
    {
        var accessToken = tokens.CreateAccessToken(user);
        var refreshToken = tokens.GenerateRefreshToken();

        db.RefreshTokens.Add(RefreshToken.Create(
            tokens.HashRefreshToken(refreshToken),
            DateTime.UtcNow.Add(tokens.RefreshTokenLifetime),
            user.Id));

        return new AuthResponse(accessToken.Token, accessToken.ExpiresAt, refreshToken, user.Role.ToString());
    }
}
