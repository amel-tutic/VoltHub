using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Auth;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Account.ChangePassword;

internal sealed class ChangePasswordCommandHandler(
    IApplicationDbContext db, ICurrentUser currentUser, IPasswordHasher passwordHasher, ITokenService tokens)
    : IRequestHandler<ChangePasswordCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        var user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
            return AccountErrors.UserNotFound;

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            return AccountErrors.WrongCurrentPassword;

        user.ChangePasswordHash(passwordHasher.Hash(request.NewPassword));
        await RefreshTokenRevocation.RevokeAllAsync(db, user.Id, cancellationToken);   // other devices must log in again
        var response = TokenIssuer.Issue(user, tokens, db);                            // this device gets a fresh pair

        await db.SaveChangesAsync(cancellationToken);   // password, revocations and the new token: one transaction
        return response;
    }
}
