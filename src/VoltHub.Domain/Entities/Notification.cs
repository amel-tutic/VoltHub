using VoltHub.Domain.Common;
using VoltHub.Domain.Enums;

namespace VoltHub.Domain.Entities;

public sealed class Notification : BaseEntity
{
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid UserId { get; private set; }

    private Notification() { }

    private Notification(Guid id, NotificationType type, string title, string message, Guid userId)
    {
        Id = id;
        Type = type;
        Title = title;
        Message = message;
        IsRead = false;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public static Notification Create(NotificationType type, string title, string message, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message is required.", nameof(message));

        return new Notification(Guid.NewGuid(), type, title.Trim(), message.Trim(), userId);
    }

    public void MarkAsRead() => IsRead = true;
}