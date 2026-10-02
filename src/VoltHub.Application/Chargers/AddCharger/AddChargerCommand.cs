using MediatR;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Chargers.AddCharger;

public sealed record AddChargerCommand(
    Guid StationId, string Code, ConnectorType ConnectorType, CurrentType CurrentType, decimal PowerKw, decimal PricePerKwh)
    : IRequest<Result<Guid>>;
