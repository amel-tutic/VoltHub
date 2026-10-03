using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Notifications.GetMyNotifications;

// SSA process 7.6: the logged-in user's notifications, newest first, plus how many are unread.
public sealed record GetMyNotificationsQuery(bool UnreadOnly, int Take = 50) : IRequest<Result<NotificationListResponse>>;
