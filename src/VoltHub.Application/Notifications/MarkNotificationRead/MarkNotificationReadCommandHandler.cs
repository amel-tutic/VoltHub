using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Notifications.MarkNotificationRead;

internal sealed class MarkNotificationReadCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<MarkNotificationReadCommand, Result>
{
    public async Task<Result> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        // Filtering by owner as well: another user's notification is simply "not found".
        var notification = await db.Notifications
            .SingleOrDefaultAsync(n => n.Id == request.Id && n.UserId == userId, cancellationToken);
        if (notification is null)
            return NotificationErrors.NotFound;

        notification.MarkAsRead();
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
