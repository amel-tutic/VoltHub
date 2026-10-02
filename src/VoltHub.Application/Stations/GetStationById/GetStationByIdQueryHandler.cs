using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Stations.GetStationById;

internal sealed class GetStationByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetStationByIdQuery, Result<StationDetailsResponse>>
{
    public async Task<Result<StationDetailsResponse>> Handle(GetStationByIdQuery request, CancellationToken cancellationToken)
    {
        var station = await db.ChargingStations.SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (station is null)
            return StationErrors.NotFound;

        var now = DateTime.UtcNow;
        var chargers = await db.Chargers
            .Where(c => c.StationId == station.Id)
            .OrderBy(c => c.Code)
            .Select(c => new
            {
                Charger = c,
                InSession = db.ChargingSessions.Any(s => s.Reservation.ChargerId == c.Id && s.Status == SessionStatus.InProgress),
                ReservedNow = db.Reservations.Any(r => r.ChargerId == c.Id && r.Status == ReservationStatus.Active &&
                                                       r.StartTime <= now && r.EndTime > now)
            })
            .ToListAsync(cancellationToken);

        var scores = await db.Ratings.Where(r => r.StationId == station.Id).Select(r => r.Score).ToListAsync(cancellationToken);

        return new StationDetailsResponse(
            station.Id, station.Name, station.Address, station.City, station.Latitude, station.Longitude, station.Description,
            scores.Count == 0 ? (double?)null : scores.Average(),
            scores.Count,
            chargers.Select(x => ChargerResponse.From(
                x.Charger, ChargerAvailability.Effective(x.Charger.Status, x.InSession, x.ReservedNow))).ToList());
    }
}
