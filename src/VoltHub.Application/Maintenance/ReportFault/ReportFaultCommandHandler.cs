using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Maintenance.ReportFault;

internal sealed class ReportFaultCommandHandler(IApplicationDbContext db) : IRequestHandler<ReportFaultCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(ReportFaultCommand request, CancellationToken cancellationToken)
    {
        var charger = await db.Chargers.SingleOrDefaultAsync(c => c.Id == request.ChargerId, cancellationToken);
        if (charger is null)
            return MaintenanceErrors.ChargerNotFound;

        var record = MaintenanceRecord.Create(MaintenanceType.Fault, request.Description, charger.Id);
        db.MaintenanceRecords.Add(record);
        charger.SetStatus(ChargerStatus.OutOfOrder);

        await db.SaveChangesAsync(cancellationToken);
        return record.Id;
    }
}
