using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Sessions.StopCharging;

public sealed record StopChargingCommand(Guid SessionId) : IRequest<Result<SessionSummaryResponse>>;
