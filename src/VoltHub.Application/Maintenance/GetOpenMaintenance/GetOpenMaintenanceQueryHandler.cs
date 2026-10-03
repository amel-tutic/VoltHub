using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Maintenance.GetOpenMaintenance;

internal sealed class GetOpenMaintenanceQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetOpenMaintenanceQuery, Result<List<OpenMaintenanceItemResponse>>>
{
    public async Task<Result<List<OpenMaintenanceItemResponse>>> Handle(GetOpenMaintenanceQuery request, CancellationToken cancellationToken)
    {
        return await db.MaintenanceRecords
            .Where(m => m.ResolvedAt == null)
            .OrderBy(m => m.Type == MaintenanceType.Fault ? 0 : 1)     // faults first, then planned services
            .ThenBy(m => m.ScheduledDate ?? m.ReportedAt)
            .Select(m => new OpenMaintenanceItemResponse(
                m.Id, m.Type, m.Description, m.ReportedAt, m.ScheduledDate,
                m.ChargerId, m.Charger.Code, m.Charger.StationId, m.Charger.Station.Name, m.Charger.Status))
            .ToListAsync(cancellationToken);
    }
}
