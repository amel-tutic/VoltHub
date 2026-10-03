using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Reports.GetStationUsageReport;

internal sealed class GetStationUsageReportQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetStationUsageReportQuery, Result<List<StationUsageReportResponse>>>
{
    public async Task<Result<List<StationUsageReportResponse>>> Handle(GetStationUsageReportQuery request, CancellationToken cancellationToken)
    {
        var (from, to) = ReportPeriod.Resolve(request.From, request.To);
        var periodHours = (to - from).TotalHours;

        var stations = await db.ChargingStations
            .Select(s => new { s.Id, s.Name, s.City, ChargerCount = db.Chargers.Count(c => c.StationId == s.Id) })
            .ToListAsync(cancellationToken);

        var sessions = await db.ChargingSessions
            .Where(s => s.Status == SessionStatus.Completed && s.StartedAt >= from && s.StartedAt < to)
            .Select(s => new
            {
                s.Reservation.Charger.StationId,
                s.StartedAt,
                EndedAt = s.EndedAt ?? s.StartedAt,
                Energy = s.EnergyKwh ?? 0m,
                Revenue = s.TotalPrice ?? 0m
            })
            .ToListAsync(cancellationToken);

        return stations
            .Select(station =>
            {
                var mine = sessions.Where(s => s.StationId == station.Id).ToList();
                var minutes = mine.Sum(s => (s.EndedAt - s.StartedAt).TotalMinutes);
                var capacityHours = station.ChargerCount * periodHours;
                return new StationUsageReportResponse(
                    station.Id, station.Name, station.City, station.ChargerCount, mine.Count,
                    mine.Sum(s => s.Energy), mine.Sum(s => s.Revenue),
                    mine.Count == 0 ? 0 : Math.Round(minutes / mine.Count, 1),
                    capacityHours <= 0 ? 0 : Math.Round(minutes / 60 / capacityHours * 100, 2));
            })
            .OrderByDescending(r => r.Sessions).ThenByDescending(r => r.EnergyKwh)
            .ToList();
    }
}
