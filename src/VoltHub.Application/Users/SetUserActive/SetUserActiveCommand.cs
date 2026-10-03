using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Users.SetUserActive;

public sealed record SetUserActiveCommand(Guid UserId, bool IsActive) : IRequest<Result>;
