using MediatR;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Chargers.SetChargerStatus;

public sealed record SetChargerStatusCommand(Guid ChargerId, ChargerStatus Status) : IRequest<Result>;
