using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Sessions.GetMySessions;

internal sealed class GetMySessionsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMySessionsQuery, Result<List<SessionHistoryResponse>>>
{
    public async Task<Result<List<SessionHistoryResponse>>> Handle(GetMySessionsQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        // Session → reservation → vehicle → user: the path that replaced the removed "direct" relationships.
        return await db.ChargingSessions
            .Where(s => s.Reservation.Vehicle.UserId == userId)
            .OrderByDescending(s => s.StartedAt)
            .Select(s => new SessionHistoryResponse(
                s.Id, s.StartedAt, s.EndedAt, s.Status,
                s.Reservation.Charger.Station.Name, s.Reservation.Charger.Code,
                s.Reservation.Vehicle.Make + " " + s.Reservation.Vehicle.Model,
                s.EnergyKwh, s.TotalPrice))
            .ToListAsync(cancellationToken);
    }
}
