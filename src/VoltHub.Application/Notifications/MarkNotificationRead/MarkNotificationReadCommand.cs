using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Notifications.MarkNotificationRead;

public sealed record MarkNotificationReadCommand(Guid Id) : IRequest<Result>;
