using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Maintenance.ResolveMaintenance;

internal sealed class ResolveMaintenanceCommandHandler(IApplicationDbContext db) : IRequestHandler<ResolveMaintenanceCommand, Result>
{
    public async Task<Result> Handle(ResolveMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var record = await db.MaintenanceRecords
            .Include(m => m.Charger)
            .SingleOrDefaultAsync(m => m.Id == request.RecordId, cancellationToken);
        if (record is null)
            return MaintenanceErrors.RecordNotFound;
        if (record.ResolvedAt is not null)
            return MaintenanceErrors.AlreadyResolved;

        record.Resolve();

        // The last open fault closed: the charger can be booked again.
        if (record.Type == MaintenanceType.Fault && record.Charger.Status == ChargerStatus.OutOfOrder)
        {
            var otherOpenFaults = await db.MaintenanceRecords.AnyAsync(m =>
                m.ChargerId == record.ChargerId && m.Id != record.Id && m.Type == MaintenanceType.Fault && m.ResolvedAt == null,
                cancellationToken);
            if (!otherOpenFaults)
                record.Charger.SetStatus(ChargerStatus.Available);
        }

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
