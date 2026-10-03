using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Reports.GetFaultReport;

internal sealed class GetFaultReportQueryHandler(IApplicationDbContext db) : IRequestHandler<GetFaultReportQuery, Result<List<FaultReportResponse>>>
{
    public async Task<Result<List<FaultReportResponse>>> Handle(GetFaultReportQuery request, CancellationToken cancellationToken)
    {
        var (from, to) = ReportPeriod.Resolve(request.From, request.To);

        var faults = await db.MaintenanceRecords
            .Where(m => m.Type == MaintenanceType.Fault && m.ReportedAt >= from && m.ReportedAt < to)
            .Select(m => new
            {
                m.ChargerId, m.ReportedAt, IsOpen = m.ResolvedAt == null,
                m.Charger.Code, StationName = m.Charger.Station.Name, m.Charger.Status
            })
            .ToListAsync(cancellationToken);

        return faults
            .GroupBy(f => new { f.ChargerId, f.Code, f.StationName, f.Status })
            .Select(g => new FaultReportResponse(
                g.Key.ChargerId, g.Key.Code, g.Key.StationName, g.Key.Status,
                g.Count(), g.Count(f => f.IsOpen), g.Max(f => (DateTime?)f.ReportedAt)))
            .OrderByDescending(r => r.Faults).ThenBy(r => r.StationName).ThenBy(r => r.ChargerCode)
            .ToList();
    }
}
