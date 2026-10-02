using MediatR;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Vehicles.UpdateVehicle;

public sealed record UpdateVehicleCommand(Guid Id, string Make, string Model, decimal BatteryCapacityKwh, ConnectorType ConnectorType)
    : IRequest<Result>;
