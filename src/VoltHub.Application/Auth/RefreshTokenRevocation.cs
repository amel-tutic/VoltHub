using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;

namespace VoltHub.Application.Auth;

// Signs a user out everywhere: every refresh token that could still be used is revoked.
// Access tokens can't be recalled, but they expire within minutes.
internal static class RefreshTokenRevocation
{
    public static async Task RevokeAllAsync(IApplicationDbContext db, Guid userId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var usable = await db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > now)
            .ToListAsync(cancellationToken);

        foreach (var token in usable)
            token.Revoke();
    }
}
