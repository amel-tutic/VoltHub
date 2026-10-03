using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Notifications.MarkAllNotificationsRead;

public sealed record MarkAllNotificationsReadCommand : IRequest<Result>;
