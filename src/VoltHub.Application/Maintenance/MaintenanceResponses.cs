using VoltHub.Domain.Enums;

namespace VoltHub.Application.Maintenance;

public sealed record MaintenanceRecordResponse(
    Guid Id, MaintenanceType Type, string Description, DateTime ReportedAt, DateTime? ScheduledDate, DateTime? ResolvedAt);

// LastServiceAt and NextServiceAt are derived from the records; they are not stored on the charger.
public sealed record ChargerMaintenanceResponse(
    Guid ChargerId, string ChargerCode, Guid StationId, string StationName, ChargerStatus Status, DateTime StatusChangedAt,
    DateTime? LastServiceAt, DateTime? NextServiceAt, IReadOnlyList<MaintenanceRecordResponse> Records);

public sealed record OpenMaintenanceItemResponse(
    Guid Id, MaintenanceType Type, string Description, DateTime ReportedAt, DateTime? ScheduledDate,
    Guid ChargerId, string ChargerCode, Guid StationId, string StationName, ChargerStatus ChargerStatus);
