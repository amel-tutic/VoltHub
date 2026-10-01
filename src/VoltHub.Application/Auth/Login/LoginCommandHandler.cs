using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Auth.Login;

internal sealed class LoginCommandHandler(IApplicationDbContext db, IPasswordHasher passwordHasher, ITokenService tokens)
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

        // Verify runs even for unknown emails, so response time doesn't reveal who is registered.
        if (!passwordHasher.Verify(request.Password, user?.PasswordHash) || user is null)
            return Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");

        if (!user.IsActive)
            return Error.Forbidden("Auth.AccountDeactivated", "This account has been deactivated.");

        var response = TokenIssuer.Issue(user, tokens, db);
        await db.SaveChangesAsync(cancellationToken);
        return response;
    }
}
