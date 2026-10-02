using VoltHub.Domain.Enums;

namespace VoltHub.Application.Vehicles;

public sealed record VehicleResponse(Guid Id, string Make, string Model, decimal BatteryCapacityKwh, ConnectorType ConnectorType);
