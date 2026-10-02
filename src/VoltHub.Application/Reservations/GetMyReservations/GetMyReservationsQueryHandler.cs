using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Reservations.GetMyReservations;

internal sealed class GetMyReservationsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyReservationsQuery, Result<List<ReservationResponse>>>
{
    public async Task<Result<List<ReservationResponse>>> Handle(GetMyReservationsQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        var query = db.Reservations.Where(r => r.Vehicle.UserId == userId);
        if (request.Status is { } status)
            query = query.Where(r => r.Status == status);

        // The navigations (r.Charger.Station.Name, r.Vehicle.Make) become SQL JOINs.
        return await query
            .OrderByDescending(r => r.StartTime)
            .Select(r => new ReservationResponse(
                r.Id, r.StartTime, r.EndTime, r.Status,
                r.ChargerId, r.Charger.Code, r.Charger.StationId, r.Charger.Station.Name,
                r.VehicleId, r.Vehicle.Make + " " + r.Vehicle.Model))
            .ToListAsync(cancellationToken);
    }
}
