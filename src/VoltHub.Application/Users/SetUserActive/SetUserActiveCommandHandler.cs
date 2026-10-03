using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Auth;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Users.SetUserActive;

// Deactivating keeps the account and its history (reservations, invoices) but blocks every login.
internal sealed class SetUserActiveCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<SetUserActiveCommand, Result>
{
    public async Task<Result> Handle(SetUserActiveCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } adminId)
            return CommonErrors.NotAuthenticated;
        if (request.UserId == adminId && !request.IsActive)
            return UserErrors.CannotDeactivateSelf;

        var user = await db.Users.SingleOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
            return UserErrors.NotFound;

        if (request.IsActive)
        {
            user.Reactivate();
        }
        else
        {
            user.Deactivate();
            await RefreshTokenRevocation.RevokeAllAsync(db, user.Id, cancellationToken);   // signed out everywhere
        }

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
