using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Maintenance.LogIntervention;

internal sealed class LogInterventionCommandHandler(IApplicationDbContext db) : IRequestHandler<LogInterventionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(LogInterventionCommand request, CancellationToken cancellationToken)
    {
        var charger = await db.Chargers.SingleOrDefaultAsync(c => c.Id == request.ChargerId, cancellationToken);
        if (charger is null)
            return MaintenanceErrors.ChargerNotFound;

        // The work is already done, so the record is created and closed in one step.
        var record = MaintenanceRecord.Create(request.Type, request.Description, charger.Id);
        record.Resolve();
        db.MaintenanceRecords.Add(record);

        if (request.Type == MaintenanceType.Repair)
        {
            var openFaults = await db.MaintenanceRecords
                .Where(m => m.ChargerId == charger.Id && m.Type == MaintenanceType.Fault && m.ResolvedAt == null)
                .ToListAsync(cancellationToken);
            foreach (var fault in openFaults)
                fault.Resolve();

            if (charger.Status is ChargerStatus.OutOfOrder or ChargerStatus.UnderMaintenance)
                charger.SetStatus(ChargerStatus.Available);
        }

        await db.SaveChangesAsync(cancellationToken);
        return record.Id;
    }
}
