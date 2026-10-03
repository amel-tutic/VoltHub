using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Notifications;

internal static class NotificationErrors
{
    public static readonly Error NotFound = Error.NotFound("Notification.NotFound", "Notification not found.");
}
