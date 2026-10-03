using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Statistics.GetMyStatistics;

internal sealed class GetMyStatisticsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyStatisticsQuery, Result<MyStatisticsResponse>>
{
    private const int Months = 6;

    public async Task<Result<MyStatisticsResponse>> Handle(GetMyStatisticsQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        // One query for the raw rows (session -> reservation -> vehicle -> owner), then simple sums in memory.
        var sessions = await db.ChargingSessions
            .Where(s => s.Reservation.Vehicle.UserId == userId && s.Status == SessionStatus.Completed)
            .Select(s => new
            {
                s.StartedAt,
                EndedAt = s.EndedAt ?? s.StartedAt,
                Energy = s.EnergyKwh ?? 0m,
                Cost = s.TotalPrice ?? 0m,
                s.Reservation.Charger.StationId,
                StationName = s.Reservation.Charger.Station.Name
            })
            .ToListAsync(cancellationToken);

        var topStations = sessions
            .GroupBy(s => new { s.StationId, s.StationName })
            .Select(g => new StationUsageResponse(g.Key.StationId, g.Key.StationName, g.Count(), g.Sum(s => s.Energy)))
            .OrderByDescending(s => s.Sessions).ThenByDescending(s => s.EnergyKwh)
            .Take(3)
            .ToList();

        // Every one of the last six months appears, even with zero sessions, so a chart has no gaps.
        var thisMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthly = Enumerable.Range(0, Months)
            .Select(i => thisMonth.AddMonths(i - Months + 1))
            .Select(month =>
            {
                var inMonth = sessions.Where(s => s.StartedAt >= month && s.StartedAt < month.AddMonths(1)).ToList();
                return new MonthlyUsageResponse(month.ToString("yyyy-MM"), inMonth.Count, inMonth.Sum(s => s.Energy), inMonth.Sum(s => s.Cost));
            })
            .ToList();

        return new MyStatisticsResponse(
            sessions.Count,
            sessions.Sum(s => s.Energy),
            sessions.Sum(s => s.Cost),
            sessions.Count == 0 ? 0 : Math.Round(sessions.Average(s => (s.EndedAt - s.StartedAt).TotalMinutes), 1),
            topStations,
            monthly);
    }
}
