using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Maintenance.PlanMaintenance;

internal sealed class PlanMaintenanceCommandHandler(IApplicationDbContext db) : IRequestHandler<PlanMaintenanceCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(PlanMaintenanceCommand request, CancellationToken cancellationToken)
    {
        if (!await db.Chargers.AnyAsync(c => c.Id == request.ChargerId, cancellationToken))
            return MaintenanceErrors.ChargerNotFound;

        var record = MaintenanceRecord.Create(
            MaintenanceType.ScheduledService, request.Description, request.ChargerId, request.ScheduledDate.UtcDateTime);
        db.MaintenanceRecords.Add(record);

        await db.SaveChangesAsync(cancellationToken);
        return record.Id;
    }
}
