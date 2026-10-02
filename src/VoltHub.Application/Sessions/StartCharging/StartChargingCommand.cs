using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Sessions.StartCharging;

public sealed record StartChargingCommand(Guid ReservationId) : IRequest<Result<Guid>>;
