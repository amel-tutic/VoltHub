using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Notifications.GetMyNotifications;

internal sealed class GetMyNotificationsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyNotificationsQuery, Result<NotificationListResponse>>
{
    public async Task<Result<NotificationListResponse>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        var mine = db.Notifications.Where(n => n.UserId == userId);
        var unreadCount = await mine.CountAsync(n => !n.IsRead, cancellationToken);

        var items = await (request.UnreadOnly ? mine.Where(n => !n.IsRead) : mine)
            .OrderByDescending(n => n.CreatedAt)
            .Take(request.Take)
            .Select(n => new NotificationResponse(n.Id, n.Type, n.Title, n.Message, n.IsRead, n.CreatedAt))
            .ToListAsync(cancellationToken);

        return new NotificationListResponse(unreadCount, items);
    }
}
