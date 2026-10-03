using VoltHub.Domain.Enums;

namespace VoltHub.Api.Contracts;

public sealed record FaultRequest(string Description);
public sealed record PlannedMaintenanceRequest(DateTimeOffset ScheduledDate, string Description);
public sealed record InterventionRequest(MaintenanceType Type, string Description);
