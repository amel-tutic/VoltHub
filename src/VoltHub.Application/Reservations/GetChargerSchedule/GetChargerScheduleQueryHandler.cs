using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Reservations.GetChargerSchedule;

internal sealed class GetChargerScheduleQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetChargerScheduleQuery, Result<List<BusySlotResponse>>>
{
    public async Task<Result<List<BusySlotResponse>>> Handle(GetChargerScheduleQuery request, CancellationToken cancellationToken)
    {
        if (!await db.Chargers.AnyAsync(c => c.Id == request.ChargerId, cancellationToken))
            return ReservationErrors.ChargerNotFound;

        var from = request.From?.UtcDateTime ?? DateTime.UtcNow;
        var to = request.To?.UtcDateTime ?? from.AddDays(7);

        return await db.Reservations
            .Where(r => r.ChargerId == request.ChargerId && r.Status == ReservationStatus.Active &&
                        r.StartTime < to && from < r.EndTime)
            .OrderBy(r => r.StartTime)
            .Select(r => new BusySlotResponse(r.StartTime, r.EndTime))
            .ToListAsync(cancellationToken);
    }
}
