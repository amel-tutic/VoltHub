using MediatR;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Vehicles.AddVehicle;

public sealed record AddVehicleCommand(string Make, string Model, decimal BatteryCapacityKwh, ConnectorType ConnectorType)
    : IRequest<Result<Guid>>;
