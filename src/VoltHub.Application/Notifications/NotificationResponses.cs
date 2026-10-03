using VoltHub.Domain.Enums;

namespace VoltHub.Application.Notifications;

public sealed record NotificationResponse(Guid Id, NotificationType Type, string Title, string Message, bool IsRead, DateTime CreatedAt);

public sealed record NotificationListResponse(int UnreadCount, IReadOnlyList<NotificationResponse> Items);
