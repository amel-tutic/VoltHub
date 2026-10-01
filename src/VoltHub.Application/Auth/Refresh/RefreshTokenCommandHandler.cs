using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Auth.Refresh;

internal sealed class RefreshTokenCommandHandler(IApplicationDbContext db, ITokenService tokens)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private static readonly Error InvalidToken =
        Error.Unauthorized("Auth.InvalidRefreshToken", "The refresh token is invalid or expired.");

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = tokens.HashRefreshToken(request.RefreshToken);
        var stored = await db.RefreshTokens
            .Include(t => t.User)
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (stored is null)
            return InvalidToken;

        if (stored.RevokedAt is not null)
        {
            // An already-used token came back: it may have been stolen.
            // Revoke all of this user's active tokens, forcing a fresh login everywhere.
            var active = await db.RefreshTokens
                .Where(t => t.UserId == stored.UserId && t.RevokedAt == null)
                .ToListAsync(cancellationToken);
            foreach (var token in active)
                token.Revoke();

            await db.SaveChangesAsync(cancellationToken);
            return InvalidToken;
        }

        if (stored.ExpiresAt <= DateTime.UtcNow || !stored.User.IsActive)
            return InvalidToken;

        stored.Revoke();   // rotation: every refresh token works exactly once
        var response = TokenIssuer.Issue(stored.User, tokens, db);
        await db.SaveChangesAsync(cancellationToken);
        return response;
    }
}
