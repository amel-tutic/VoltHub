using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Maintenance;

internal static class MaintenanceErrors
{
    public static readonly Error ChargerNotFound = Error.NotFound("Maintenance.ChargerNotFound", "Charger not found.");
    public static readonly Error RecordNotFound = Error.NotFound("Maintenance.RecordNotFound", "Maintenance record not found.");
    public static readonly Error AlreadyResolved = Error.Conflict("Maintenance.AlreadyResolved", "This record is already resolved.");
}
