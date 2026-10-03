using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Maintenance.GetChargerMaintenance;

internal sealed class GetChargerMaintenanceQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetChargerMaintenanceQuery, Result<ChargerMaintenanceResponse>>
{
    public async Task<Result<ChargerMaintenanceResponse>> Handle(GetChargerMaintenanceQuery request, CancellationToken cancellationToken)
    {
        var charger = await db.Chargers
            .Where(c => c.Id == request.ChargerId)
            .Select(c => new { c.Id, c.Code, c.StationId, StationName = c.Station.Name, c.Status, c.StatusChangedAt })
            .SingleOrDefaultAsync(cancellationToken);
        if (charger is null)
            return MaintenanceErrors.ChargerNotFound;

        var records = await db.MaintenanceRecords
            .Where(m => m.ChargerId == charger.Id)
            .OrderByDescending(m => m.ReportedAt)
            .Select(m => new MaintenanceRecordResponse(m.Id, m.Type, m.Description, m.ReportedAt, m.ScheduledDate, m.ResolvedAt))
            .ToListAsync(cancellationToken);

        // Derived, never stored: the latest completed service and the earliest service still planned.
        var lastServiceAt = records
            .Where(r => r.Type != MaintenanceType.Fault && r.ResolvedAt != null)
            .Max(r => r.ResolvedAt);
        var nextServiceAt = records
            .Where(r => r.Type == MaintenanceType.ScheduledService && r.ResolvedAt == null && r.ScheduledDate != null)
            .Min(r => r.ScheduledDate);

        return new ChargerMaintenanceResponse(
            charger.Id, charger.Code, charger.StationId, charger.StationName, charger.Status, charger.StatusChangedAt,
            lastServiceAt, nextServiceAt, records);
    }
}
